$(document).ready(function () {

    if ($('#validationSummary').text().trim() == "") {
        $('#validationSummary').hide();
    }

    $('.btnSubmitForm').on('click', function () {
        if ($('#validationSummary').text().trim() != "") {
            $('#validationSummary').show();
        }
    });

});