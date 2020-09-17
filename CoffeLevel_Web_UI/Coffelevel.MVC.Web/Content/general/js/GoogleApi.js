 
$(window).load(function () { $("#spinnerLoading").fadeOut(4000); })

function initialize() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (pos) {

            var userLatLng = new google.maps.LatLng(pos.coords.latitude, pos.coords.longitude);
            var mapCanvas = document.getElementById('map_canvas');
            var mapOptions = {
                center: userLatLng,
                zoom: 13,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };

            var mapObject = new google.maps.Map(mapCanvas, mapOptions);
            new google.maps.Marker({ map: mapObject, position: userLatLng });


        }, function () {
            alert("Konumunuz bulunamadı !");
        });
    } else {
        alert("Kullandığınız browser GeoLocation API yi desteklemiyor ! ");
    }
     
}

google.maps.event.addDomListener(window, 'load', initialize);
