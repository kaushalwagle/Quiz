// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function checkResult(id, baseUrl) {
    if ($('input[name = "option"]').is(":checked")) {
        var option = $('input[name="option"]:checked').val();
        var urlCheck = baseUrl+'/Quiz/CheckAnswer?id=' + id + '&option=' + option;
        $.ajax({
            type: 'GET',
            url: urlCheck,
            dataType: 'json',
            success: function (jsonData) {
                var res = jsonData;
                $('#btnCheck').removeClass("btn-primary");
                $('input[name="option"]').parent().removeClass("btn-info");
                $('input[name="option"]').addClass("disabled");
                $('input[name="option"]').parent().addClass("btn-secondary");

                if (res == "Correct") {
                    $('#btnCheck').addClass("btn btn-success");
                    $('input[name="option"]:checked').parent().removeClass("btn-secondary").addClass("btn btn-success disabled");
                } else {
                    $('#btnCheck').addClass("btn btn-danger");
                    $('input[name="option"]:checked').parent().removeClass("btn-secondary").addClass("btn btn-danger disabled");
                }

                $('#btnCheck').text("Next");
                $('#btnCheck').removeAttr("onClick");
                $('#btnCheck').attr("href", baseUrl+"/Quiz");

            },
            error: function (error) {
                result.text(error);
            }
        });
    } else {
        alert("Please select an answer!");
    }
}