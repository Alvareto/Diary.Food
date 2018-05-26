(function ($) {

    var _mealService = abp.services.app.meal;
    var _$modal = $('#MealEditModal');
    var _$form = $('form[name=MealEditForm]');

    function save() {

        if (!_$form.valid()) {
            return;
        }

        var meal = _$form.serializeFormToObject(); //serializeFormToObject is defined in main.js
        meal.ingredients = [];
        var _$ingredientCheckboxes = $("input[name='ingredient']:checked");
        if (_$ingredientCheckboxes) {
            for (var ingredientIndex = 0; ingredientIndex < _$ingredientCheckboxes.length; ingredientIndex++) {
                var _$ingredientCheckbox = $(_$ingredientCheckboxes[ingredientIndex]);
                meal.ingredients.push(_$ingredientCheckbox.attr('data-ingredient-name'));
            }
        }

        delete meal.ingredient;
        console.log(meal);

        abp.ui.setBusy(_$form);
        _mealService.updateWithNames(meal).done(function () {
            _$modal.modal('hide');
            location.reload(true); //reload page to see edited meal!
        }).always(function () {
            abp.ui.clearBusy(_$modal);
        });
    }

    //Handle save button click
    _$form.closest('div.modal-content').find(".save-button").click(function (e) {
        e.preventDefault();
        save();
    });

    //Handle enter key
    _$form.find('input').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            save();
        }
    });

    $.AdminBSB.input.activate(_$form);

    _$modal.on('shown.bs.modal', function () {
        _$form.find('input[type=text]:first').focus();
    });
})(jQuery);