(function ($) {

    var _ingredientService = abp.services.app.ingredient;
    var _$modal = $('#IngredientEditModal');
    var _$form = $('form[name=IngredientEditForm]');

    function save() {

        if (!_$form.valid()) {
            return;
        }

        var ingredient = _$form.serializeFormToObject(); //serializeFormToObject is defined in main.js
        console.log(ingredient);

        ingredient.NutritionFacts = [];
        var _$factTxtInputs = $("input[name='efact']");
        if (_$factTxtInputs) {
            for (var factIndex = 0; factIndex < _$factTxtInputs.length; factIndex++) {
                var _$factTxtInput = $(_$factTxtInputs[factIndex]);

                var nutritionFact = {};
                nutritionFact.Nutrient = _$factTxtInput.attr('data-fact-id');
                nutritionFact.Nutrient = _$factTxtInput.attr('data-fact-name');
                nutritionFact.Value = _$factTxtInput.val();

                ingredient.NutritionFacts.push(nutritionFact);
            }
        }

        console.log(ingredient);

        abp.ui.setBusy(_$form);
        _ingredientService.update(ingredient).done(function () {
            _$modal.modal('hide');
            location.reload(true); //reload page to see edited ingredient!
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