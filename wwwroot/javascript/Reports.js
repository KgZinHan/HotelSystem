/* Reports Js functions */

function selectReport() {

    var keyword = $('#inputKeyword').val();

    if (keyword == null || keyword == "") {
        keyword = "";
    }

    $('#spanSearchingText').show();

    $.ajax({
        type: 'GET',
        url: "/Reports/ShowReportForm",
        data: { reportNo: keyword },
        success: function (data) {
            $('#homeBody').html(data);
            $('#spanSearchingText').hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 401) {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            } else {
                alert('Error in searching keyword.');
                $('#spanSearchingText').hide();
            }
        }
    });
}



