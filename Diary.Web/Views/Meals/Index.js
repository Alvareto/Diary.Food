(function () {
    $(function () {

        var _mealService = abp.services.app.meal;
        var _$modal = $('#MealCreateModal');
        var _$form = _$modal.find('form');

        //_$form.validate({
        //    rules: {
        //        Password: "required",
        //        ConfirmPassword: {
        //            equalTo: "#Password"
        //        }
        //    }
        //});

        $('#RefreshButton').click(function () {
            refreshMealList();
        });

        $('.delete-meal').click(function () {
            var mealId = $(this).attr("data-meal-id");
            var mealName = $(this).attr('data-meal-name');

            deleteMeal(mealId, mealName);
        });

        $('.edit-meal').click(function (e) {
            var mealId = $(this).attr("data-meal-id");

            e.preventDefault();
            $.ajax({
                url: abp.appPath + 'Meals/EditMealModal?mealId=' + mealId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#MealEditModal div.modal-content').html(content);
                },
                error: function (e) { console.log(e); }
            });
        });

        _$form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

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

            abp.ui.setBusy(_$modal);
            _mealService.createWithNames(meal).done(function () {
                _$modal.modal('hide');
                location.reload(true); //reload page to see new meal!
            }).always(function () {
                abp.ui.clearBusy(_$modal);
            });
        });

        _$modal.on('shown.bs.modal', function () {
            _$modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshMealList() {
            location.reload(true); //reload page to see new meal!
        }

        function deleteMeal(mealId, mealName) {
            abp.message.confirm(
                "Delete meal '" + mealName + "'?",
                function (isConfirmed) {
                    if (isConfirmed) {
                        _mealService.delete({
                            id: mealId
                        }).done(function () {
                            refreshMealList();
                        });
                    }
                }
            );
        }

        $(".date-picker").each(function () {
            var $datepicker = $(this);
            const cur_date = ($datepicker.data("date") ? moment($datepicker.data("date"), "YYYY/MM/DD") : moment());
            var format = {
                "weekday": ($datepicker.find(".weekday").data("format")
                    ? $datepicker.find(".weekday").data("format")
                    : "dddd"),
                "date": ($datepicker.find(".date").data("format") ? $datepicker.find(".date").data("format") : "MMMM Do"),
                "year": ($datepicker.find(".year").data("year") ? $datepicker.find(".weekday").data("format") : "YYYY")
            };

            function updateDisplay(cur_date) {
                

                $datepicker.find(".date-container > .weekday").text(cur_date.format(format.weekday));
                $datepicker.find(".date-container > .date").text(cur_date.format(format.date));
                $datepicker.find(".date-container > .year").text(cur_date.format(format.year));
                $datepicker.data("date", cur_date.format("YYYY/MM/DD"));
                $datepicker.find(".input-datepicker").removeClass("show-input");
            }

            updateDisplay(cur_date);

            $datepicker.on("click",
                '[data-toggle="datepicker"]',
                function (event) {
                    //event.preventDefault();
                    abp.ui.block();

                    var cur_date = moment($(this).closest(".date-picker").data("date"), "YYYY/MM/DD");
                    const date_type = ($datepicker.data("type") ? $datepicker.data("type") : "days");
                    const type = ($(this).data("type") ? $(this).data("type") : "add");
                    const amt = ($(this).data("amt") ? $(this).data("amt") : 1);

                    if (type == "add") {
                        cur_date = cur_date.add(date_type, amt);
                    } else if (type == "subtract") {
                        cur_date = cur_date.subtract(date_type, amt);
                    }

                    updateDisplay(cur_date);
                    //refreshMealList();
                });

            if ($datepicker.data("keyboard") === true) {
                $(window).on("keydown",
                    function (event) {
                        if (event.which == 37) {
                            $datepicker.find("span:eq(0)").trigger("click");
                        } else if (event.which == 39) {
                            $datepicker.find("span:eq(1)").trigger("click");
                        }
                    });
            }

        });
    });
})();