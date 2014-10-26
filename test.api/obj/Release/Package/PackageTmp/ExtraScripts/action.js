$(document).ready(function () {

    //fetch users
    $('#useHighestCard').click(function () {
        $('#showUsers').empty();
        $('#pleaseWait').show();
        $('#useHighestCard').disable = true;
        
        var checked = $('#useHighestCard').checked;
        if (!checked) {
            $.getJSON('/biller/getallusers', function(data) {

                $('#showUsers').append('<br/>');
                $.each(data, function(i, user) {
                    var userData = user.Name + ' [ ' + user.EmailAddress + ' ]';
                    $('#showUsers').append($('<li/>', { text: userData }));
                });
            });
        } else {
            $('#showUsers').empty();
            $('#showUsers').text('not checked');
        }
        
        $('#pleaseWait').hide();
        $('#useHighestCard').enable = true;
    });

    $("#IsRequiredMessage").hide();

    $('#dd_card_brand').change(function () {
        var selectedItem = $('#dd_card_brand').val();

        var sParts = selectedItem.split("|");
        var firstPart = sParts[0];
        var secondPart = sParts[1];
        var thirdPart = sParts[2];
        var fourthPart = sParts[3];
        var fifthPart = sParts[4];
        var sixthPart = sParts[5];
        var canRedeem = sParts[6];

        if (canRedeem == 1) {
            $('#canRedeem').show();
        } else {
            $('#canRedeem').hide();
        }

        if (sixthPart == 0) {
            $('#IsRequiredMessage').show();
            $('#divProvideLoginInfo').hide();
        } else {
            $('#IsRequiredMessage').hide();
            $('#divProvideLoginInfo').show();
        }
    });

})