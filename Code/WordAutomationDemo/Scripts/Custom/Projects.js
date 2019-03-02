$(document).ready(function () {

    if ($('#validationSummary').text().trim() == "") {
        $('#validationSummary').hide();
    }

    $('.btnSubmitForm').on('click', function () {
        if ($('#validationSummary').text().trim() != "") {
            $('#validationSummary').show();
        }
    });

    $('#projectDocuments').multiselect({
        includeSelectAllOption: true,
        enableFiltering: true
    });

    $('#selectMembers').multiselect({
        includeSelectAllOption: true,
        enableFiltering: true
    });

});

