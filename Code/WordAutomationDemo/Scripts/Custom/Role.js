$(function () {
    $("#Role").focus();
    $("#Role").change(function () {
        IsRoleExists();
    });
    $("#Company_ID").change(function () {
        IsRoleExists();
    });
});

function IsRoleExists() {
    if ($("#Company_ID").val().trim().length > 0) {
        if ($("#Role").val().trim().length > 0) {
            var SiteBaseUrl = SiteUrl + "Role/ValidateDuplicateRole";
            var data = { RoleID: $("#RoleID").val(), Role: $("#Role").val(), companyID: $("#Company_ID").val() };
            $.post(SiteBaseUrl, data, function (result) {
                if (result.status == "0") {
                    $("#duplicateRecord").show();
                    $("#btnSubmit").prop("disabled", true);
                }
                else {
                    $("#duplicateRecord").hide();
                    $("#btnSubmit").prop("disabled", false);
                }
            }, "json");
        }
    }
    else {
        alert("Please select company to add role.")
        $("#Role").val('');
        return;
    }
}

$(document).ready(function () {

    if ($('#validationSummary').text().trim() == "") {
        $('#validationSummary').hide();
    }

    $('.btnSubmitForm').on('click', function () {
        if ($('#validationSummary').text().trim() != "") {
            $('#validationSummary').show();
        }
    });

    $('#dropdownCompany').multiselect({
        includeSelectAllOption: true,
        enableFiltering: true
    });

});