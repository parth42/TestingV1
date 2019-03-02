$(document).ready(function () {
    $('#chkViewAll').click(function () {
        $(this).parents('table').find('INPUT[name=\'chkView\']').prop('checked', $(this).is(':checked'));
        //if ($(this).is(':checked') == false) {
        //    $('#chkAddAll').prop('checked', false);
        //    $(this).parents('table').find('INPUT[name=\'chkAdd\']').prop('checked', false);
        //    $('#chkEditAll').prop('checked', false);
        //    $(this).parents('table').find('INPUT[name=\'chkEdit\']').prop('checked', false);
        //    $('#chkDeleteAll').prop('checked', false);
        //    $(this).parents('table').find('INPUT[name=\'chkDelete\']').prop('checked', false);
        //    $('#chkDetailAll').prop('checked', false);
        //    $(this).parents('table').find('INPUT[name=\'chkDetail\']').prop('checked', false);
        //}
    });
});

$(document).ready(function () {
    $('#chkAddAll').click(function () {
        $(this).parents('table').find('INPUT[name=\'chkAdd\']').prop('checked', $(this).is(':checked'));
        if ($(this).is(':checked')) {
            $(this).parents('table').find('INPUT[name=\'chkView\']').prop('checked', true);
            $('#chkViewAll').prop('checked', true);
        }
    });
});

$(document).ready(function () {
    $('#chkEditAll').click(function () {
        $(this).parents('table').find('INPUT[name=\'chkEdit\']').prop('checked', $(this).is(':checked'));
        if ($(this).is(':checked')) {
            $(this).parents('table').find('INPUT[name=\'chkView\']').prop('checked', true);
            $('#chkViewAll').prop('checked', true);
        }
    });
});

$(document).ready(function () {
    $('#chkDeleteAll').click(function () {
        $(this).parents('table').find('INPUT[name=\'chkDelete\']').prop('checked', $(this).is(':checked'));
        if ($(this).is(':checked')) {
            $(this).parents('table').find('INPUT[name=\'chkView\']').prop('checked', true);
            $('#chkViewAll').prop('checked', true);
        }
    });
});

$(document).ready(function () {
    $('#chkDetailAll').click(function () {
        $(this).parents('table').find('INPUT[name=\'chkDetail\']').prop('checked', $(this).is(':checked'));
        if ($(this).is(':checked')) {
            $(this).parents('table').find('INPUT[name=\'chkView\']').prop('checked', true);
            $('#chkViewAll').prop('checked', true);
        }
    });
});

function UnSelectHeaderCheckbox(itemClassName, headerCheckBox) {
    var totalChecked = $(".grid-tab tr td." + itemClassName).find("input[type='checkbox']:checked").length;
    var totalCheckbox = $(".grid-tab tr td." + itemClassName).find("input[type='checkbox']").length;
    if (totalCheckbox == totalChecked) {
        $("#" + headerCheckBox).prop('checked', true);
    } else {
        $("#" + headerCheckBox).prop('checked', false);
    }
}

function UncheckParentCheckbox(childClassName, parentClassName, parentID, obj) {
    var parClass = parentClassName + parentID;
    if (obj.checked) {
        $(".grid-tab tr").find("input." + parClass + ":checkbox").each(function () {
            $(this).prop('checked', true);
        });
    }
    //else {
    //    var childClass = childClassName + parentID;
    //    var totalChecked = $(".grid-tab tr").find("input." + childClass + "[type='checkbox']:checked").length;
    //    if (totalChecked === 0) {
    //        $(".grid-tab tr").find("input." + parClass + ":checkbox").each(function () {
    //            $(this).prop('checked', false);
    //        });
    //    }
    //}
}

function UncheckMainParentCheckbox(MainParentID, obj) {
    if (!obj.checked) {
        var MainParent = document.getElementById(MainParentID)

        if (MainParent != null) {
            MainParent.checked = false;
        }
    }
}

//Check/Uncheck checkbox of the parent
function CheckUncheckParentCheckBox(ParentClassName) {
    if (ParentClassName != "") {
        $(".grid-tab tr").find("input:checkbox").each(function () {
            if ($(this).attr('class') == ParentClassName) {
                $(this).prop('checked', true);
            }
        });
    }
}

function CheckUncheckViewChildCheckBox(obj, parentID) {
    var parentStatus = obj.checked;
    var childView = "chkView" + parentID;
    var childAdd = "chkAdd" + parentID;
    var childEdit = "chkEdit" + parentID;
    var childDelete = "chkDelete" + parentID;
    var childDetail = "chkDetail" + parentID;

    var childAddPar = "chkAddPar" + parentID;
    var childEditPar = "chkEditPar" + parentID;
    var childDeletePar = "chkDeletePar" + parentID;
    var childDetailPar = "chkDetailPar" + parentID;

    if (obj.checked) {
        $(".grid-tab tr").find("input:checkbox").each(function () {
            if ($(this).attr('class') == childView || $(this).attr('class') == childAdd || $(this).attr('class') == childEdit || $(this).attr('class') == childDelete || $(this).attr('class') == childDetail
            ||
            $(this).attr('class') == childView || $(this).attr('class') == childAddPar || $(this).attr('class') == childEditPar || $(this).attr('class') == childDeletePar || $(this).attr('class') == childDetailPar) {
                $(this).prop('checked', true);
            }
        });
    }
    else {
        $(".grid-tab tr").find("input." + childView + ":checkbox").each(function () {
            $(this).prop('checked', false);
        });
    }


    UnSelectHeaderCheckbox('chkAdd', 'chkAddAll');
    UnSelectHeaderCheckbox('chkEdit', 'chkEditAll');
    UnSelectHeaderCheckbox('chkDelete', 'chkDeleteAll');
    UnSelectHeaderCheckbox('chkDetail', 'chkDetailAll');
}

function CheckUncheckChildCheckBox(obj, childClassName, parentID) {
    var parentStatus = obj.checked;
    var childClass = childClassName + parentID;
    $(".grid-tab tr").find("input." + childClass + ":checkbox").each(function () {
        $(this).prop('checked', parentStatus);
    });

    if (parentStatus == true) {
        var parView = "chkViewPar" + parentID;
        $(".grid-tab tr").find("input." + parView + ":checkbox").each(function () {
            $(this).prop('checked', true);
        });
        var childView = "chkView" + parentID;
        $(".grid-tab tr").find("input." + childView + ":checkbox").each(function () {
            $(this).prop('checked', true);
        });
    }
    UnSelectHeaderCheckbox('chkView', 'chkViewAll');
}

//Check/Uncheck checkbox of the Child
function CheckUncheckChildCheckBoxNew(ParentID, Type, Control) {
    if (ParentID != "") {
        $(".grid-tab tr").find("input:checkbox").each(function () {

            var obj = $(this);
            if (obj[0].id != null) {
                var arr = obj[0].id.split('_');
                if (arr.length > 1) {
                    if (arr[1] == ParentID && arr[0] == Type)
                        $(this).prop('checked', Control.checked);
                }
            }
        });
    }
}

function getPrivileges(id) {
    document.forms[0].submit();
}
function SelectViewDetail(strMenuItemId, obj, parentID) {
    if (obj.checked) {
        $("input[value=v" + strMenuItemId + "]").prop('checked', true);
        $("input[value=v" + parentID + "]").prop('checked', true);
    }
    UnSelectHeaderCheckbox('chkView', 'chkViewAll');
}

function CheckUncheckRowCheckBox(obj, parentID) {
    var childAdd = "chkAdd" + parentID;
    var childEdit = "chkEdit" + parentID;
    var childDelete = "chkDelete" + parentID;
    var childDetail = "chkDetail" + parentID;

    //var childchkAll = "chkAll" + menuItemID;
    if ($(obj).is(':checked') == false) {
        var trObject = $(obj).parent("td").parent("tr");
        $(trObject).find("input:checkbox").each(function () {
            if ($(this).attr('class') == childAdd || $(this).attr('class') == childEdit || $(this).attr('class') == childDelete || $(this).attr('class') == childDetail) {
                $(this).prop('checked', false);
            }
        });
    }
}
