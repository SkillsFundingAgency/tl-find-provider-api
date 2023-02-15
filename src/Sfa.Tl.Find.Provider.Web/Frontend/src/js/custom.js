$(document).ready(function () {
    
    $(".notification-close").click(
        function () {
            $(this).parents(".tl-notification").fadeOut();
        }
    );

});