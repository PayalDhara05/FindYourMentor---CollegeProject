$(document).ready(function () {

    var API_Key = "AIzaSyCiULU2UqIB-hPuG6Uxpv0Tx6tTCOHRFdU";
    var video = " ";

    $('.form-control').keydown(function (event) {
        // enter has keyCode = 13, change it if you want to use another button
        if (event.keyCode == 13) {
            // this.form.submit();
            $("#video-form").submit();
            return false;
        }
    });

    $("#video-form").submit(function (e) {
        e.preventDefault();

        var search = $("#search").val();
        $("#videos").html("");

        youtubeSearch(API_Key, search, 12)
    })

    function youtubeSearch(key, search, maxResult) {
        $.get("https://www.googleapis.com/youtube/v3/search?key=" + key + "&part=snippet&type=video&q=" + search + "&maxResults=" + maxResult, function (data) {
            data.items.forEach(item => {
                video = `
                    <iframe width="420" height="315" src="https://www.youtube.com/embed/${item.id.videoId}" frameborder="0" allowfullscreen></iframe>
                    `

                //$("#videos").empty().append(video);
                $("#videos").append(video);
            })
        });
    }

    $("#clear-btn").click(function () {
        $("#videos").html("");
    })
})