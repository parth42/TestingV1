function ValidateDuplicateCompany(pagename, fieldName, textboxID, primaryKeyField, MessageFieldName, PrimaryKeyFieldName) {
    
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
                if (MessageFieldName == "Company" && primaryKeyField == "add" && strValueList != "") {
                    $("#SuccessMessage").html(_CompanyNameAvailable);
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
                if (MessageFieldName == "Company") {
                    $("#ErrorMessage").html(_CompanyNameNotAvailable);
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