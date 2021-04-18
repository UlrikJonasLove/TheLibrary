// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    function hideOnLoad() {
        $("#ThePages, #TheRunTime, #Borrowable,#TheCeo, #TheManager, #TheSalary, #TheManagerId").hide()
    }

    hideOnLoad();

    $("#Types select").change(function () {
        var value = $(this).val();

        $("div.form-group").each(function () {
            $(this).show()
        })

        if (value == "Book")
            $("#TheRunTime").hide();
        else if (value === "AudioBook" || value === "DVD")
            $("#ThePages").hide();
        else if (value == "ReferenceBook")
            $("#TheRunTime, #Borrowable, #TheDate, #TheBorrower").hide();
        else if (value == "RegularEmployee") {
            $("#TheManager, #TheCeo").hide();
            $("#pId").text("Add an Manager that will Manage this Employee");
        }
        else if (value == "Manager") {
            $("#TheCeo").hide();
            $("#pId").text("Add an Manager that will Manage this Manager");
        }
        else if (value == "Ceo") {
            $("#TheManager").hide();
            $("#pId").text("Add an Manager that this CEO will Manage");
        }
        else
            hideOnLoad();

        $("div.form-group").each(function () {
            if ($(this).is("visible"))
                $(this).children("input").attr("required");
            else
                $(this).children("input").removeAttr("required");
        })
    })


});