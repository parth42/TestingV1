$(document).ready(function() {
    /* $('.navbar-toggle').click(function () {
		$('.prjlink').prop("href","https://stackoverflow.com/questions/16562577/how-can-i-make-a-button-redirect-my-page-to-another-page");
		alert("click")
		$('.prjlink').prop("has-submenu");
		$('.prjlink').prop("highlighted");	
	});*/
	
	/*$('[data-toggle=popover]').popover({
    content: $('.PopoverContent').html(),
    html: true
}).onmouseover(function() {
    $(this).popover('show');
});*/

	
	
});

///Popover
$("[data-toggle=popover]").each(function(i, obj) {

$(this).popover({
  html: true,
  content: function() {
    var id = $(this).attr('id')
    return $('#popover-content-' + id).html();
  }
});

});

$(window).off("resize").on("resize", function() {
    $(".popover").each(function() {
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
})


///Brows file Button
$(function() {

  // We can attach the `fileselect` event to all file inputs on the page
  $(document).on('change', ':file', function() {
    var input = $(this),
        numFiles = input.get(0).files ? input.get(0).files.length : 1,
        label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
    input.trigger('fileselect', [numFiles, label]);
  });

  // We can watch for our custom `fileselect` event like this
  $(document).ready( function() {
      $(':file').on('fileselect', function(event, numFiles, label) {

          var input = $(this).parents('.input-group').find(':text'),
              log = numFiles > 1 ? numFiles + ' files selected' : label;

          if( input.length ) {
              input.val(log);
          } else {
              if( log ) alert(log);
          }

      });
  });
  
});


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




