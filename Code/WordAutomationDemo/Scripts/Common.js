$(function () {
    var buttonvalue;
    var buttonid;
    var buttonname;

    $('button[type="button"]').click(function () {
        buttonvalue = $(this).attr('value');
        buttonid = $(this).attr('id');
        buttonname = $(this).attr('name');
        $('#' + buttonid).attr("disabled", "disabled");
    })

})
