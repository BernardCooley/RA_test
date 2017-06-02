$(function () {
    $("#dialog-modal").dialog({
        autoOpen: false,
        show: {
            effect: "blind",
            duration: 500
        },
        hide: {
            effect: "blind",
            diration: 500
        },
        height: 600,
        width: 700,
        modal: true
    });

    $(".artists").mouseover(function () {
        var artist = $(this).data('artist-name');
        console.log("artist hovered");
        $.ajax({
            url: "GetArtist",
            type: "get",
            data: { artistName: artist },
            success: function (data) {
                displayArtist(data);
            }
        })
    });

});

var displayArtist = function (artist) {
    $('#profileImageUrl').attr('src', artist.profileImageURL);
    $('#artistName').text(artist.name);
    $('#country').text(artist.country);
    $('#website').attr('href', artist.website);
    $('#website').text(artist.website);
    $('#labels').text(artist.labels);

    $('#raProfile').attr('href', artist.raProfile);
    $('#raProfile').text(artist.raProfile);

    $('#twitter').attr('href', 'https://twitter.com/' + artist.twitter);
    $('#twitter').text(artist.twitter);

    $('#facebook').attr('href', 'https://facebook.com/' + artist.facebook);
    $('#facebook').text(artist.facebook);

    $('#discogs').attr('href', 'https://www.discogs.com/artist/' + artist.discogs);
    $('#discogs').text(artist.discogs);

    $('#biosmall').text(artist.bioSmall);
    $("#dialog-modal").dialog("open");
}