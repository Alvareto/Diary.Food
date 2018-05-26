(function () {
    $(function () {

        var _ingredientService = abp.services.app.ingredient;
        var _$modal = $('#IngredientCreateModal');
        var _$form = _$modal.find('form');

        $('#RefreshButton').click(function () {
            refreshIngredientList();
        });

        $('.delete-ingredient').click(function () {
            var ingredientId = $(this).attr("data-ingredient-id");
            var ingredientName = $(this).attr('data-ingredient-name');

            deleteIngredient(ingredientId, ingredientName);
        });

        $('.edit-ingredient').click(function (e) {
            var ingredientId = $(this).attr("data-ingredient-id");

            e.preventDefault();
            $.ajax({
                url: abp.appPath + 'Ingredients/EditIngredientModal?ingredientId=' + ingredientId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#IngredientEditModal div.modal-content').html(content);
                },
                error: function (e) { console.log(e); }
            });
        });

        _$form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!_$form.valid()) {
                return;
            }

            var ingredient = _$form.serializeFormToObject(); //serializeFormToObject is defined in main.js
            console.log(ingredient);


            ingredient.NutritionFacts = [];
            var _$factTxtInputs = $("input[name='fact']");
            if (_$factTxtInputs) {
                for (var factIndex = 0; factIndex < _$factTxtInputs.length; factIndex++) {
                    var _$factTxtInput = $(_$factTxtInputs[factIndex]);

                    var nutritionFact = {};
                    nutritionFact.Nutrient = _$factTxtInput.attr('data-fact-name');
                    nutritionFact.Value = _$factTxtInput.val();

                    ingredient.NutritionFacts.push(nutritionFact);
                }
            }

            //delete ingredient.ingredient;
            console.log(ingredient);

            abp.ui.setBusy(_$modal);
            _ingredientService.create(ingredient).done(function () {
                _$modal.modal('hide');
                location.reload(true); //reload page to see new ingredient!
            }).always(function () {
                abp.ui.clearBusy(_$modal);
            });
        });

        _$modal.on('shown.bs.modal', function () {
            _$modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshIngredientList() {
            location.reload(true); //reload page to see new ingredient!
        }

        function deleteIngredient(ingredientId, ingredientName) {
            abp.message.confirm(
                "Delete ingredient '" + ingredientName + "'?",
                function (isConfirmed) {
                    if (isConfirmed) {
                        _ingredientService.delete({
                            id: ingredientId
                        }).done(function () {
                            refreshIngredientList();
                        });
                    }
                }
            );
        }

    });
})();