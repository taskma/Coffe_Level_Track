



$(document).ready(function () {
  

    
    //$('#coffeeStatus').bind('click', function()
    //{
    //    $(function () {
    //        window.alert("basıldı2");

    //    });
    //})

  
           var bilgiGonder = false;
           var bilgiGonderInt = 1;
           var degisken = null;
           var yanipSonKahveDurumdegisken = null;
           var buton = null;
           $("#waterSpan1").removeClass("waterA1");
           $("#waterSpan2").removeClass("waterA2");
           $("#waterSpan3").removeClass("waterA3");
           $("#waterSpan4").removeClass("waterA4");
           $("#waterSpan5").removeClass("waterA5");
           if ($("#span5").hasClass("coffeeA5"))
               $("#span5").removeClass("coffeeA5");
           if ($("#span4").hasClass("coffeeA4"))
               $("#span4").removeClass("coffeeA4");
           if ($("#span3").hasClass("coffeeA3"))
               $("#span3").removeClass("coffeeA3");
           if ($("#span2").hasClass("coffeeA2"))
               $("#span2").removeClass("coffeeA2");
           if ($("#span1").hasClass("coffeeA1"))
               $("#span1").removeClass("coffeeA1");
           $(function repeatMe() {
               $.ajax({
                   url: '/CoffeeWeb/CoffeLevelGetir',
                   type: 'POST',
                   dataType: 'json',
                   success: function (data) {
                       $.getJSON("/CoffeeWeb/CoffeLevelGetir", null, function (data) {
                           $('#sonKayitZaman').html(data.time);
                         $('#numberOfDesire').html(data.totalDesire);
                  
                         window.clearInterval(yanipSonKahveDurumdegisken);
                           $('#coffeeStatus').html(data.coffeeStatus);

                           if (data.coffeeStatus == "Cihaz Fişi Takılı Değil") {
                           yanipSonKahveDurumdegisken = setInterval(function yanipSonKahveDurum() {

                              
                                   if ($("#coffeeStatus").css("visibility") == "visible") {
                                       $("#coffeeStatus").css("visibility", "hidden");
                                   }
                                   else {
                                       $("#coffeeStatus").css("visibility", "visible");
                                   }
                              
                              
                           }, 700);
                           }
                           else {
                            
                               $("#coffeeStatus").css("visibility", "visible");

                       
                       }




                           if (data.coffeeStatus != "Kahve yok istekte bulunabilirsin") {
                               $('#desiredButon').hide();
                           }
                           else {
                               $('#desiredButon').show();
                           }



                           if (data.level == 5 && data.isCooking)
                           {
                               $('#remainingLevel').html("4,5/5");
                           }
                           else
                           {
                               $('#remainingLevel').html(data.level + "/5");
                           }

                           if (bilgiGonder) {
                              // window.clearInterval(buton);

                               //    jQuery.each(data, function (i, eleman) {
                               //  $("#LevelId").css({ 'font-size': '80px' });

                               //   $('#LevelId').html(data.level + ". seviyede şu anda. ");
                               //var element = document.getElementById("span1");
                               //window.alert(data.level);
                               if (data.isOpen)
                               {
                                   //buton = setInterval(function () {

                                   //    if ($("#spanRedGreenButton").css("visibility") == "visible") {
                                   //        $("#spanRedGreenButton").css("visibility", "hidden");
                                   //    }
                                   //    else {
                                   //        $("#spanRedGreenButton").css("visibility", "visible");
                                   //    }
                                   //}, 700);
                        
                                   if (!$("#spanRedGreenButton").hasClass("gauge-red")) {
                                       $("#spanRedGreenButton").removeClass("gauge-red");
                                   }
                                   $("#spanRedGreenButton").addClass("gauge-green");
                               }
                               else
                               {
                                   if (!$("#spanRedGreenButton").hasClass("gauge-green")) {
                                       $("#spanRedGreenButton").removeClass("gauge-green");
                                   }
                                   $("#spanRedGreenButton").addClass("gauge-red");
                               }
                               if (data.isCooking) {

                                   $("#spanSmoke").addClass("smoke");
                                   $("#spanSmoke2").addClass("smoke2");
                                   $("#spanSmoke3").addClass("smoke3");
                                   $("#spanSmoke4").addClass("smoke4");
                                   $("#spanDamlama").addClass("coffee-drip");
                                   $("#spanDamlama2").addClass("drip-cover");
                                  
                                   switch (data.level) {
                                       case 0:
                                           $("#waterSpan1").addClass("waterA1");
                                           $("#waterSpan2").addClass("waterA2");
                                           $("#waterSpan3").addClass("waterA3");
                                           $("#waterSpan4").addClass("waterA4");
                                           $("#waterSpan5").addClass("waterA5");
                                           break;
                                       case 1:
                                           if (!$("#span1").hasClass("coffeeA1")) {
                                               $("#span1").addClass("coffeeA1");
                                           }
                                           $("#waterSpan1").addClass("waterA1");
                                           $("#waterSpan2").addClass("waterA2");
                                           $("#waterSpan3").addClass("waterA3");
                                           $("#waterSpan4").addClass("waterA4");
                                           if ($("#waterSpan5").hasClass("waterA5"))
                                               $("#waterSpan5").removeClass("waterA5");
                                           break;
                                       case 2:
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           $("#waterSpan1").addClass("waterA1");
                                           $("#waterSpan2").addClass("waterA2");
                                           $("#waterSpan3").addClass("waterA3");
                                           $("#waterSpan4").removeClass("waterA4");
                                           $("#waterSpan5").removeClass("waterA5");
                                           break;
                                       case 3:
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           if (!$("#span3").hasClass("coffeeA3"))
                                               $("#span3").addClass("coffeeA3");
                                           $("#waterSpan1").addClass("waterA1");
                                           $("#waterSpan2").addClass("waterA2");
                                           $("#waterSpan3").removeClass("waterA3");
                                           $("#waterSpan4").removeClass("waterA4");
                                           $("#waterSpan5").removeClass("waterA5");
                                           break;
                                       case 4:
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           if (!$("#span3").hasClass("coffeeA3"))
                                               $("#span3").addClass("coffeeA3");
                                           if (!$("#span4").hasClass("coffeeA4"))
                                               $("#span4").addClass("coffeeA4");
                                           $("#waterSpan1").addClass("waterA1");
                                           $("#waterSpan2").removeClass("waterA2");
                                           $("#waterSpan3").removeClass("waterA3");
                                           $("#waterSpan4").removeClass("waterA4");
                                           $("#waterSpan5").removeClass("waterA5");
                                           break;
                                       case 5:
                                           window.clearInterval(degisken);
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           if (!$("#span3").hasClass("coffeeA3"))
                                               $("#span3").addClass("coffeeA3");
                                           if (!$("#span4").hasClass("coffeeA4"))
                                               $("#span4").addClass("coffeeA4");
                                           if (!$("#span5").hasClass("coffeeA5"))
                                               $("#span5").addClass("coffeeA5");
                                           $("#waterSpan1").removeClass("waterA1");
                                           $("#waterSpan2").removeClass("waterA2");
                                           $("#waterSpan3").removeClass("waterA3");
                                           $("#waterSpan4").removeClass("waterA4");
                                           $("#waterSpan5").removeClass("waterA5");

                                           degisken = setInterval(function yanipSon() {
                                              
                                               if ($("#span5").css("visibility") == "visible")
                                               {
                                                   $("#span5").css("visibility", "hidden");
                                               } 
                                               else 
                                               {
                                                   $("#span5").css("visibility", "visible");
                                               } 
                                           },700);  

                                           break;
                                   }
                               }
                               else {
                                   window.clearInterval(degisken);
                                   $("#span5").css("visibility", "visible");

                                   if (data.level > 0)
                                   {
                                           if ($("#sec").is(':checked')) {
                                               var myWindow = window.open("", "MsgWindow", "width=200,height=100,top=500,left=1000");
                                               myWindow.document.write("<p>Kahve Hazır!!!!!</p>");
                                               document.getElementById("sec").checked = false
                                           }

                                   }
                          
                                   $("#spanSmoke").removeClass("smoke");
                                   $("#spanSmoke2").removeClass("smoke2");
                                   $("#spanSmoke3").removeClass("smoke3");
                                   $("#spanSmoke4").removeClass("smoke4");
                                   $("#spanDamlama").removeClass("coffee-drip");
                                   $("#spanDamlama2").removeClass("drip-cover");
                                   if ($("#waterSpan1").hasClass("waterA1"))
                                       $("#waterSpan1").removeClass("waterA1");
                                   if ($("#waterSpan2").hasClass("waterA2"))
                                       $("#waterSpan2").removeClass("waterA2");
                                   if ($("#waterSpan3").hasClass("waterA3"))
                                       $("#waterSpan3").removeClass("waterA3");
                                   if ($("#waterSpan4").hasClass("waterA4"))
                                       $("#waterSpan4").removeClass("waterA4");
                                   if ($("#waterSpan5").hasClass("waterA5"))
                                       $("#waterSpan5").removeClass("waterA5");
                                   switch (data.level) {
                                       case 5:
                                          
                                           if (!$("#span5").hasClass("coffeeA5"))
                                               $("#span5").addClass("coffeeA5");
                                           if (!$("#span4").hasClass("coffeeA4"))
                                               $("#span4").addClass("coffeeA4");
                                           if (!$("#span3").hasClass("coffeeA3"))
                                               $("#span3").addClass("coffeeA3");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");

                                           break;
                                       case 4:
                                           if ($("#span5").hasClass("coffeeA5"))
                                               $("#span5").removeClass("coffeeA5");
                                           if (!$("#span4").hasClass("coffeeA4"))
                                               $("#span4").addClass("coffeeA4");
                                           if (!$("#span3").hasClass("coffeeA3"))
                                               $("#span3").addClass("coffeeA3");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           break;
                                       case 3:
                                           if ($("#span5").hasClass("coffeeA5"))
                                               $("#span5").removeClass("coffeeA5");
                                           if ($("#span4").hasClass("coffeeA4"))
                                               $("#span4").removeClass("coffeeA4");
                                           if (!$("#span3").hasClass("coffeeA3"))
                                               $("#span3").addClass("coffeeA3");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           break;
                                       case 2:
                                           if ($("#span5").hasClass("coffeeA5"))
                                               $("#span5").removeClass("coffeeA5");
                                           if ($("#span4").hasClass("coffeeA4"))
                                               $("#span4").removeClass("coffeeA4");
                                           if ($("#span3").hasClass("coffeeA3"))
                                               $("#span3").removeClass("coffeeA3");
                                           if (!$("#span2").hasClass("coffeeA2"))
                                               $("#span2").addClass("coffeeA2");
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           break;
                                       case 1:
                                           if ($("#span5").hasClass("coffeeA5"))
                                               $("#span5").removeClass("coffeeA5");
                                           if ($("#span4").hasClass("coffeeA4"))
                                               $("#span4").removeClass("coffeeA4");
                                           if ($("#span3").hasClass("coffeeA3"))
                                               $("#span3").removeClass("coffeeA3");
                                           if ($("#span2").hasClass("coffeeA2"))
                                               $("#span2").removeClass("coffeeA2");
                                           if (!$("#span1").hasClass("coffeeA1"))
                                               $("#span1").addClass("coffeeA1");
                                           break;
                                       case 0:
                                           if ($("#span5").hasClass("coffeeA5"))
                                               $("#span5").removeClass("coffeeA5");
                                           if ($("#span4").hasClass("coffeeA4"))
                                               $("#span4").removeClass("coffeeA4");
                                           if ($("#span3").hasClass("coffeeA3"))
                                               $("#span3").removeClass("coffeeA3");
                                           if ($("#span2").hasClass("coffeeA2"))
                                               $("#span2").removeClass("coffeeA2");
                                           if ($("#span1").hasClass("coffeeA1"))
                                               $("#span1").removeClass("coffeeA1");
                                           break;
                                   }
                               }
                           }
                       }
                           );
                       setTimeout(repeatMe, 8000);
                       if (bilgiGonderInt == 1)
                       {
                           bilgiGonderInt = 2;
                       }
                       else
                       {
                           bilgiGonder = true;
                       }
                       


                       //});
                   },
                   error: function () {
                       setTimeout(repeatMe, 8000);
                   }
               });
           }
           )
       })

