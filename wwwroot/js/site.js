
$("#mainSwitcher").click(function(){
    $('input:checkbox').not(this).prop('checked', this.checked);
});

$("#myInput").on("keyup", function () {
    var value = $(this).val().toLowerCase();
    $(".searchable").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
});
