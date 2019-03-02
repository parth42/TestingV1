jQuery(document).ready(function () {
    $('#Password').keyup(
                function () {
                    $('#passComment').html(passwordStrength($('#Password').val(), $('#Username').val()));
                    if ($('#passComment').html() == "&nbsp;&nbsp;Too short" || $('#passComment').html() == "&nbsp;&nbsp;Average") {
                        document.getElementById("passComment").style.color = 'red';
                    }
                    else {
                        document.getElementById("passComment").style.color = 'green';
                    }
                }
            )

    $('#SMTPEmailPassword').keyup(
                function () {
                    $('#passSMTPComment').html(passwordStrength($('#SMTPEmailPassword').val(), $('#SMTPEmailAddress').val()));
                    if ($('#passSMTPComment').html() == "&nbsp;&nbsp;Too short" || $('#passComment').html() == "&nbsp;&nbsp;Average") {
                        document.getElementById("passSMTPComment").style.color = 'red';
                    }
                    else {
                        document.getElementById("passSMTPComment").style.color = 'green';
                    }
                }
            )
});

$(document).ready(function () {
    document.getElementById("UserName").focus();
});



$(function () {
    $("#Password").change(function () {
        if (document.getElementById('Password').value.length > 0) {
            document.getElementById("spanConfirm").innerHTML = _NotConfirmed;//'Not Confirmed';
            document.getElementById("spanConfirm").style.color = 'red';
        }
        else {
            document.getElementById("spanConfirm").innerHTML = '';
        }
        return false;
    });
    $("#SMTPEmailPassword").change(function () {
        if (document.getElementById('SMTPEmailPassword').value.length > 0) {
            document.getElementById("spanSMTPConfirm").innerHTML = _NotConfirmed;//'Not Confirmed';
            document.getElementById("spanSMTPConfirm").style.color = 'red';
        }
        else {
            document.getElementById("spanSMTPConfirm").innerHTML = '';
        }
        return false;
    });
});

function funCofirmPassword() {
    //if (document.getElementById('Password').value.length > 0 && document.getElementById('ConfirmPassword').value.length > 0 && document.getElementById('Password').value != document.getElementById('ConfirmPassword').value) {
    //    document.getElementById("spanConfirm").innerHTML = _PasswordNotMatch;//'Passwords do not match.';
    //    document.getElementById("spanConfirm").style.color = 'red';
    //}
    //else if (document.getElementById('Password').value.length > 0 && document.getElementById('ConfirmPassword').value.length == 0) {
    //    document.getElementById("spanConfirm").innerHTML = _NotConfirmed;//'Not Confirmed';
    //    document.getElementById("spanConfirm").style.color = 'red';
    //}
    //else {
    //    document.getElementById("spanConfirm").innerHTML = _Confirmed;//'Confirmed';
    //    document.getElementById("spanConfirm").style.color = 'green';
    //}
    if ($("#Password").valid() && $("#ConfirmPassword").valid()) {
        if (document.getElementById('Password').value.length == 0 || document.getElementById('ConfirmPassword').value.length == 0) {
            document.getElementById("spanConfirm").innerHTML = "";
        }
        else if (document.getElementById('Password').value.length > 0 && document.getElementById('ConfirmPassword').value.length > 0 && document.getElementById('Password').value != document.getElementById('ConfirmPassword').value) {
            document.getElementById("spanConfirm").innerHTML = "";
        }
        else {
            document.getElementById("spanConfirm").innerHTML = _Confirmed;//'Confirmed';
            document.getElementById("spanConfirm").style.color = 'green';
        }
    }
    else {
        document.getElementById("spanConfirm").innerHTML = "";
    }    
}

function funSMTPCofirmPassword() {
    if (document.getElementById('SMTPEmailPassword').value.length > 0 && document.getElementById('SMTPEmailConfirmPassword').value.length > 0 && document.getElementById('SMTPEmailPassword').value != document.getElementById('SMTPEmailConfirmPassword').value) {
        document.getElementById("spanSMTPConfirm").innerHTML = _PasswordNotMatch;//'Passwords do not match.';
        document.getElementById("spanSMTPConfirm").style.color = 'red';
    }
    else if (document.getElementById('SMTPEmailPassword').value.length > 0 && document.getElementById('SMTPEmailConfirmPassword').value.length == 0) {
        document.getElementById("spanSMTPConfirm").innerHTML = _NotConfirmed;//'Not Confirmed';
        document.getElementById("spanSMTPConfirm").style.color = 'red';
    }
    else {
        document.getElementById("spanSMTPConfirm").innerHTML = _Confirmed;//'Confirmed';
        document.getElementById("spanSMTPConfirm").style.color = 'green';
    }
}

function checkusername() {
    var data = { UserName: $("#Username").val(), UserID: $('#UserID').val() };
    var SiteBaseUrl = SiteUrl + "User/validateDuplicateRecord";
    $.post(SiteBaseUrl, data, function (data) {
        if (data.status == "0") {
            document.getElementById("duplicateRecord").style.display = '';
            document.getElementById("btnSubmit").disabled = true;
        }
        else {
            document.getElementById("duplicateRecord").style.display = 'none';
            document.getElementById("btnSubmit").disabled = false;
        }
    }, "json");
}

function ValidateDuplicateUser(pagename, fieldName, textboxID, primaryKeyField, MessageFieldName, PrimaryKeyFieldName) {
    pagename = SiteUrl + pagename;
    var fieldNameArry = fieldName.split(",");
    var textBoxIDArry = textboxID.split(",");

    var strFieldList = "";
    var strValueList = "";

    for (var i = 0; i < fieldNameArry.length; i++) {
        if (strFieldList != "") {

            strFieldList += ",";
            strValueList += ",";
        }
        strFieldList += fieldNameArry[i];
        strValueList += $("#" + textBoxIDArry[i]).val();
    }
    var strEditID = -1;
    if (primaryKeyField != "add")
        strEditID = $("#" + primaryKeyField).val();
    if (strValueList.length > 0) {
        $.post(pagename,
    { FieldList: strFieldList, ValueList: strValueList, strAddOrEditID: strEditID, IDName: PrimaryKeyFieldName },
        function (data) {
            var myObject = eval(data);
            var newid;
            if (myObject.status != undefined)
                newid = myObject.status;
            else
                newid = myObject;
            if (newid == 1) {
                if (MessageFieldName == "User" && primaryKeyField == "add" && strValueList != "") {
                    $("#SuccessMessage").html(_UsernameAvailable);
                    $('#SuccessMessage').css('display', "");
                    $("#ErrorMessage").html("");
                    $('#ErrorMessage').css('display', "none");
                }
                else {
                    $("#SuccessMessage").html("");
                    $('#SuccessMessage').css('display', "none");
                    $("#ErrorMessage").html("");
                    $('#ErrorMessage').css('display', "none");
                }
                return true;
            }
            else if (newid == -1) {
                $("#ErrorMessage").html("");
                $('#ErrorMessage').css('display', "none");
            }
            else {
                if (MessageFieldName == "User") {
                    $("#ErrorMessage").html(_UsernameNotAvailable);
                    $('#ErrorMessage').css('display', "");
                    $("#SuccessMessage").html("");
                    $('#SuccessMessage').css('display', "none");
                }
                else {
                    //$("#ErrorMessage").html("<ul><li>" + MessageFieldName + " already exists. Try to choose different value. </ul></li>");
                    $("#ErrorMessage").html(_AlreadyExists.replace("##FieldName##", MessageFieldName));
                    $('#ErrorMessage').css('display', "");
                    $("#SuccessMessage").html("");
                    $('#SuccessMessage').css('display', "none");
                }
                return false;
            }
        });
    }
}