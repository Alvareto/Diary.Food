using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Diary.Authorization.Roles;
using Diary.Domain;
using Diary.Domain.Dto;
using Diary.Domain.Models;
using Stateless;

namespace Diary.Workflow
{
    public interface IHostNotification
    {
        void Notify(string message, IngredientDto task);
        void NotifyTaskStarted(string workflowInstanceId, int taskId);
        void NotifyTaskChanged(int taskId);
        void NotifyDelayPassed(int taskId);
    }
    public delegate void TaskStartedDelegate(string workflowInstanceId, int taskId);
    public delegate void TaskChangedDelegate(int taskId);
    public delegate void DelayPassedDelegate(int taskId);
    public delegate void MessageReceivedDelegate(string message, IngredientDto task);

    public enum WfType
    {
        Synchronous, Bookmark, Service
    }

    public class HostEventNotifier : IHostNotification
    {
        public event TaskStartedDelegate TaskStarted;
        public event TaskChangedDelegate TaskChanged;
        public event DelayPassedDelegate DelayPassed;
        public event MessageReceivedDelegate MessageReceived;

        public void NotifyTaskStarted(string workflowInstanceId, int taskId)
        {
            if (TaskStarted != null)
            {
                // State => Preda kod metode koji će se izvršiti kad thread postane aktivan
                ThreadPool.QueueUserWorkItem((state) => TaskStarted(workflowInstanceId, taskId), null);
            }
        }

        public void NotifyTaskChanged(int taskId)
        {
            if (TaskChanged != null)
            {
                ThreadPool.QueueUserWorkItem((state) => TaskChanged(taskId), null);
            }
        }

        public void NotifyDelayPassed(int taskId)
        {
            if (DelayPassed != null)
            {
                ThreadPool.QueueUserWorkItem((state) => DelayPassed(taskId), null);
            }
        }

        public void Notify(string message, IngredientDto task)
        {
            if (MessageReceived != null)
            {
                ThreadPool.QueueUserWorkItem((state) => MessageReceived(message, task), null);
            }
        }
    }

    public sealed class NotifyHost : CodeActivity
    {
        // svojstva vidljiva na dijagramu
        public InArgument<string> Message { get; set; }

        public InArgument<bool> TaskStarted { get; set; }
        public InArgument<bool> TaskChanged { get; set; }
        public InArgument<bool> DelayPassed { get; set; }
        public InArgument<IngredientDto> Task { get; set; }

        // pretpostavljena implementacija apstraktne metode
        protected override void Execute(CodeActivityContext context)
        {
            // metode za obradu događaja registriramo ekstenzijom nad sučeljem koje smo sami definirali
            IHostNotification extension = context.GetExtension<IHostNotification>();

            IngredientDto task = Task.Get(context);

            if (TaskChanged.Get(context))  // vrijednost argumenta/svojstva iz konteksta
            {
                extension.NotifyTaskChanged(task.Id);
            }
            if (TaskStarted.Get(context))
            {
                extension.NotifyTaskStarted(context.WorkflowInstanceId.ToString(), task.Id);
            }
            if (DelayPassed.Get(context))
            {
                extension.NotifyDelayPassed(task.Id);
            }

            string message = Message?.Get(context); // bilo koja poruka
            if (!string.IsNullOrEmpty(message))
            {
                extension.Notify(message, task);
            }
        }
    }

    // vlastity aktivnost za kreiranje bookmarka
    public class WaitForHost : NativeActivity<Task>
    {
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }    // property za dizajn protoka

        protected override void Execute(NativeActivityContext context)
        {
            Debug.WriteLine("{0:HH:mm:ss} - Bookmark {1} Execute", DateTime.Now, BookmarkName);
            // ime oznake postaljeno u dizajnu protoka, npr. TaskWorkFlow \ WaitForHost \ "Oznaka"
            context.CreateBookmark(BookmarkName.Get(context), OnResumeBookmark);
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            Debug.WriteLine("{0:HH:mm:ss} - Bookmark {1} Resume", DateTime.Now, BookmarkName);
            Result.Set(context, (Task)obj);
        }

        // Gets or sets a value that indicates whether the activity can cause the workflow to become idle.
        protected override bool CanInduceIdle => true;
    }

    public class ApprovalProcess
        : IEventHandler<EntityCreatedEventData<Ingredient>>,
        IEventHandler<EntityUpdatedEventData<Ingredient>>,
            ITransientDependency
    {
        private enum ApprovalProcessTrigger
        {
            Accept,
            Reject
        }

        public enum ApprovalProcessState
        {
            Pending,
            Approved,
            Rejected
        }

        public ApprovalProcessState State => _stateMachine.State;
        public IngredientDto Ingredient { get; private set; }

        private readonly StateMachine<ApprovalProcessState, ApprovalProcessTrigger> _stateMachine;

        private readonly StateMachine<ApprovalProcessState, ApprovalProcessTrigger>.TriggerWithParameters<IngredientDto> _approveTrigger;

        private readonly StateMachine<ApprovalProcessState, ApprovalProcessTrigger>.TriggerWithParameters<IngredientDto> _rejectTrigger;

        private readonly IIngredientAppService _service;

        public ApprovalProcess(IIngredientAppService service, IngredientDto ingredient)
        {
            _service = service;
            Ingredient = ingredient;

            _stateMachine = new StateMachine<ApprovalProcessState, ApprovalProcessTrigger>(ApprovalProcessState.Pending);
            _approveTrigger = _stateMachine.SetTriggerParameters<IngredientDto>(ApprovalProcessTrigger.Accept);
            _rejectTrigger = _stateMachine.SetTriggerParameters<IngredientDto>(ApprovalProcessTrigger.Reject);

            ConfigureStateMachine();
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.Configure(ApprovalProcessState.Pending)
                .Permit(ApprovalProcessTrigger.Accept, ApprovalProcessState.Approved)
                .Permit(ApprovalProcessTrigger.Reject, ApprovalProcessState.Rejected);
            _stateMachine.Configure(ApprovalProcessState.Approved)
                .OnEntryFrom(_approveTrigger, OnApproved);
            _stateMachine.Configure(ApprovalProcessState.Rejected)
                .OnEntryFrom(_rejectTrigger, OnRejected);
        }

        private void OnRejected(IngredientDto dto)
        {
            dto.Status = ApprovalStatus.Rejected;

            _service.Reject(dto.Id);
            //// update ingredient in DB
            //_service.Update(dto);
            //// delete ingredient in DB
            //_service.Delete(dto);
        }

        private void OnApproved(IngredientDto dto)
        {
            dto.Status = ApprovalStatus.Approved;
            // update ingredient in DB
            _service.Update(dto);
        }

        public void Approve()
        {
            Approve(Ingredient);
        }

        public void Approve(IngredientDto dto)
        {
            _stateMachine.Fire(_approveTrigger, dto);
        }

        public void Reject()
        {
            Reject(Ingredient);
        }

        public void Reject(IngredientDto dto)
        {
            _stateMachine.Fire(_rejectTrigger, dto);
        }

        public bool Equals(ApprovalProcess anotherProcess)
        {
            return State == anotherProcess.State && Ingredient.Name == anotherProcess.Ingredient.Name;
        }

        public void HandleEvent(EntityCreatedEventData<Ingredient> eventData)
        {
            var entity = eventData.Entity;

            // on created
            //Approve(entity);
        }

        public void HandleEvent(EntityUpdatedEventData<Ingredient> eventData)
        {
            throw new NotImplementedException();
        }
    }
}
