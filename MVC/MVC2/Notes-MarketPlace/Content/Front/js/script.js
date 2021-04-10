/*********************
    Javascript For all Front Pages.
*********************/

/** Password text hide/show toggler on eye image **/
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

/** HomePage Navbar change on scroll **/

$(function () {
    showHidenav();
    $(window).scroll(function () {
        showHidenav();
    });

    function showHidenav() {
        if ($(window).scrollTop() > 50) {

            //show White navbar
            $("#nav").addClass("bg-light");
            $("ul.navbar-nav > li > a").css("color", "#333");

            //Home Page - Dark logo

            $("#home-logo img").attr("src", "/Content/Front/images/user-profile/navbarbanner.png");

        } else {
            $("#nav").removeClass("bg-light");
            $("ul#home-list > li > a").css("color", "#fff");
            $("#home-logo img").attr("src", "/Content/Front/images/top-logo.png");
        }
    }
});

// FAQ page event of hide/show answers

$(document).ready(function () {
    $('.card .collapse').on('shown.bs.collapse', function () {
        $(this).parent().find('img').attr('src', '../../Content/Front/images/FAQ/minus.png');
        $(this).parent().find('.card-header').css('background-color', '#fff').css('border-bottom', 'none').css('background-position', 'center');
    });
    $('.card .collapse').on('hidden.bs.collapse', function () {
        $(this).parent().find('img').attr('src', '../../Content/Front/images/FAQ/add.png');
        $(this).parent().find('.card-header').css('background-color', 'rgba(0,0,0,.03)');;
    });
});

// Toggler Navbar Buttton for responsive page
$(document).ready(function () {
    $('.navbar-collapse').on('shown.bs.collapse', function () {
        $('.navbar-toggler-icon').css("background", "none")
        $('.navbar-toggler-icon').html("&times;").css("font-size", "45px")
        $('nav').css('height', '100%').css('display', 'block');
        $(window).resize(function () {
            if ($(window).width() >= 992) {
                $('.navbar-collapse').collapse('hide');
            }
        })

        if (($(window).scrollTop() < 50)) {
            $("#nav").addClass("bg-light");
            $("ul.navbar-nav > li > a").css("color", "#333");

            //dark logo

            $("#home-logo img").attr("src", "/Content/Front/images/user-profile/navbarbanner.png");
        }

    });
    $('.navbar-collapse').on('hidden.bs.collapse', function () {

        $('.navbar-toggler-icon').html("&#9776;").css("font-size", "35px")
        $('nav').css('height', '66px');

        if (($(window).scrollTop() < 50)) {
            $("#nav").removeClass("bg-light");
            $("ul#home-list > li > a").css("color", "#fff");
            // normal logo
            $("#home-logo img").attr("src", "/Content/Front/images/top-logo.png");

        }
    });
})