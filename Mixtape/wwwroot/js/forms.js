// Help for forms.
//
// data-enable-with="id": the field can only be used while the radio button with that id is chosen
// (the name of a new mixtape, when moving a song). The page already sends it enabled or disabled;
// this keeps it right when the choice changes.
(function () {
    'use strict';

    document.addEventListener('change', function (e) {
        var field = e.target.form && e.target.form.querySelector('[data-enable-with]');

        if (field) {
            field.disabled = !document.getElementById(field.getAttribute('data-enable-with')).checked;
        }
    });
})();
