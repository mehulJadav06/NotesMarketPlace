$(function () {

    $(".toggle-password").click(function () {

        $(this).toggleClass("eye");

        var input = $($(this).attr("toggle"));
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });
})

$(document).ready(function () {

    $(window).resize(function () {
        if ($(window).width() >= 992) {
            $('.navbar-collapse').collapse('hide');
        }
    })
    /* FAQ Page  */
    $('.card .collapse').on('shown.bs.collapse', function () {
        $(this).parent().find('img').attr('src', 'images/FAQ/minus.png');
        $(this).parent().find('.card-header').css('background-color', '#fff').css('border-bottom', 'none');
    });
    $('.card .collapse').on('hidden.bs.collapse', function () {
        $(this).parent().find('img').attr('src', 'images/FAQ/add.png');
        $(this).parent().find('.card-header').css('background-color', 'rgba(0,0,0,.03)');;
    });
});


$(document).ready(function () {
    $('.navbar-collapse').on('shown.bs.collapse', function () {
        $('.navbar-toggler-icon').html("&times;").css("font-size", "45px")

        $('nav').css('height', '100%').css('display', 'block');


    });
    $('.navbar-collapse').on('hidden.bs.collapse', function () {

        $('.navbar-toggler-icon').html("&#9776;").css("font-size", "35px")
        $('nav').css('height', '73px');
    });
})
