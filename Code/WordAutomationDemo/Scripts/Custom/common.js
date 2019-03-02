function initializeAllCheckBox() {
    var n = $("#allCheckBox").is(":checked"); $(".singleCheckBox").prop("checked", n ? !0 : !1); $(".singleCheckBox").closest("tr").addClass(n ? "selected-row" : "not-selected-row"); $(".singleCheckBox").closest("tr").removeClass(n ? "not-selected-row" : "selected-row")
}
function initializeSingleCheckBox(n) {
    var t = $(n).is(":checked"); $(n).closest("tr").addClass(t ? "selected-row" : "not-selected-row"); $(n).closest("tr").removeClass(t ? "not-selected-row" : "selected-row"); t && $(".singleCheckBox").length == $(".selected-row").length ? $("#allCheckBox").prop("checked", !0) : $("#allCheckBox").prop("checked", !1)
}
function Check_CheckBox_Count() {
    var n = $("input[name=chkDelete]"), t = n.filter(":checked").length; return t > 0 ? confirm("Are you sure? Do you want to delete record(s)?") : (alert("Please select at least one checkbox to delete."), !1)
}


function validateEmail(n) {
    return n.match(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/) ? !0 : !1
}
function ValidateNumber(n) {
    var i = window.event || n || event, t = i.which || i.keyCode; return t > 31 && (t < 48 || t > 57) && (t < 96 || t > 105) ? !1 : !0
} function isNumberKey(n, t, i, r) {
    var o = GetCursorLocation(n), u = t.which ? t.which : t.keyCode, e, s, f; if (u > 31 && (u < 48 || u > 57) && (u < 96 || u > 105) && u != 46 && u != 190 && u != 110 && !(u == 8 || u == 16 || u == 32 || u == 35 || u == 36 || u == 37 || u == 39 || u == 46)) return !1; if (e = i, s = r, Num = n.value, f = Num.indexOf("."), f != -1) { if (u != 9 && u != 8 && Num.length - f > s && (o > f || f > e - 1) || f > e - 1 && o < f && u != 8 && u != 9) return !1 } else if (u != 9 && u != 8 && Num.length > e - 1 && u != 46) return !1; return !0
} function isNumberOnlyKey(n, t) {
    var u = GetCursorLocation(n), i = t.which ? t.which : t.keyCode, r = t.keyCode; return i > 95 && i < 106 ? !0 : i > 31 && (i < 46 || i > 57) && i != 47 && r != 9 && r != 37 && r != 38 && r != 39 && r != 40 ? !1 : i == 47 ? !1 : !0
} function GetCursorLocation(n) {
    var t, i, u, r = -1; return typeof n.selectionStart == "number" ? r = n.selectionStart : document.selection && n.createTextRange && (t = document.selection, t && (u = t.createRange(), i = n.createTextRange(), i.setEndPoint("EndToStart", u), r = i.text.length)), r
} function populateDropdown(n, t, i) {
    n.html(""); n.append($("<option><\/option>").val("").html(i)); $.each(t, function (t, i) { n.append($("<option><\/option>").val(i.value).html(i.name)) }); $(n).removeAttr("disabled")
} function populateDropdownWithoutLabel(n, t) { $(n).children("option:not(:first)").remove(); $.each(t, function (t, i) { n.append($("<option><\/option>").val(i.value).html(i.name)) }); $(n).removeAttr("disabled") } function onGridDataBound(n) { if (!n.sender.dataSource.view().length) { var t = n.sender.thead.find("th:visible").length, i = '<tr><td colspan="' + t + '" align="center">No records found.<\/td><\/tr>'; n.sender.tbody.parent().width(n.sender.thead.width()).end().html(i) } } function validateFromDateToDate(n, t) { var i = n.split("/"), u = parseInt(i[0]), f = parseInt(i[1]), e = parseInt(i[2]), r = t.split("/"), o = parseInt(r[0]), s = parseInt(r[1]), h = parseInt(r[2]), c = new Date(u + "/" + f + "/" + e), l = new Date(o + "/" + s + "/" + h); return c > l ? !1 : !0 } function getQueryString(n) { for (urlStr = window.location.search.substring(1), sv = urlStr.split("&"), i = 0; i < sv.length; i++) if (ft = sv[i].split("="), ft[0] == n) return ft[1] } function DisableCutCopyPaste() { $("input[type='password']").bind("cut copy paste", function (n) { n.preventDefault() }) } var dateFomat = '@WordAutomationDemo.Helpers.CurrentUserSession.User.DTformat', MaskingFormatPhone = "(000) 000-0000", MaskingFormatZip = "00000", MaskingFormatLicense = "000000000", MaskingFormatFifteenDigit = "000000000000000", MaskingFormatNineDigit = "000000000", _ValidFileExtention = "You can only upload files like .xls, .xlsx.", _RequiredField = "##FieldName## is required.", _StartFileProcess = "File process starting...", _ValidationCompleted = "Validation process has been completed and starting to process file...", _RecordCount = "Validating records ##MsgRecordCount## out of ##MsgTotalRecord##", _ErrorValidating = "Some error occurred while validating file...", _FileProcessCompleted = "File process has been completed....", _FileProcessing = "Processing records ##MsgRecordCount## out of ##MsgTotalRecord##", _ErrorProcessing = "Some error occurred while processing file...", _AjaxSuccess = "1", _AjaxFailure = "0", ww, adjustMenu; $.validator.setDefaults({ ignore: [] }); $(document).on("change", ".btn-file :file", function () { var n = $(this), t = n.get(0).files ? n.get(0).files.length : 1, i = n.val().replace(/\\/g, "/").replace(/.*\//, ""); n.trigger("fileselect", [t, i]) }); $(document).ready(function () { $(".btn-file :file").on("fileselect", function (n, t, i) { var u = $(this).parents(".input-group").find(":text"), r = t > 1 ? t + " files selected" : i; u.length ? u.val(r) : r && alert(r) }); DisableCutCopyPaste() }); ww = $(window).width(); $(window).load(function () { $("#main-menu li a").each(function () { $(this).next().length > 0 && $(this).addClass("parent") }); $(".toggleMenu").click(function (n) { n.preventDefault(); $(this).toggleClass("active"); $("#main-menu").toggle() }); adjustMenu() }); $(window).bind("resize orientationchange", function () { ww = $(window).width(); adjustMenu() }); adjustMenu = function () { ww < 1023 ? ($(".toggleMenu").css("display", "inline-block"), $(".toggleMenu").hasClass("active") ? $("#main-menu").show() : $("#main-menu").hide(), $("#main-menu li").unbind("mouseenter mouseleave"), $("#main-menu li a.parent").unbind("click").bind("click", function (n) { n.preventDefault(); $(this).parent("li").toggleClass("hover") })) : ww >= 768 && ($(".toggleMenu").css("display", "none"), $("#main-menu").show(), $("#main-menu li").removeClass("hover"), $("#main-menu li a").unbind("click"), $("#main-menu li").unbind("mouseenter mouseleave").bind("mouseenter mouseleave", function () { $("#main-menu li").hoverIntent({ over: function () { $(this).removeClass("hover").addClass("hover") }, out: function () { $(this).removeClass("hover") } }) })) }; $("#main-menu").bind("show.smapi", function (n, t) { $(t).dataSM("parent-a").children("span.sub-arrow").text("-") }); $("#main-menu").bind("hide.smapi", function (n, t) { $(t).dataSM("parent-a").children("span.sub-arrow").text("+") })

///Popover
$("[data-toggle=popover]").each(function (i, obj) {
    $(this).popover({
        html: true,
        content: function () {
            var id = $(this).attr('id')
            return $('#popover-content-' + id).html();
        }
    });

});

$(window).off("resize").on("resize", function () {
    $(".popover").each(function () {
        var popover = $(this);
        if (popover.is(":visible")) {
            var ctrl = $(popover.context);
            ctrl.popover('show');
        }
    });
});

///Tooltip 
$(function () {
    $('[data-toggle="tooltip"]').tooltip()

    // We can attach the `fileselect` event to all file inputs on the page
    $(document).on('change', ':file', function () {
        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger('fileselect', [numFiles, label]);
    });

    $(':file').on('fileselect', function (event, numFiles, label) {
        var input = $(this).parents('.input-group').find(':text'),
            log = numFiles > 1 ? numFiles + ' files selected' : label;

        if (input.length) {
            input.val(log);
        }

    });

    // We can watch for our custom `fileselect` event like this
    //$(document).ready(function () {
        
    //});

})

///Modal popup
/*$(function() {
function reposition() {
var modal = $(this),
dialog = modal.find('.modal-dialog');
modal.css('display', 'block');

dialog.css("margin-top", Math.max(0, ($(window).height() - dialog.height()) / 2));
}

$('.modal').on('show.bs.modal', reposition);

$(window).on('resize', function() {
$('.modal:visible').each(reposition);
});

});*/




