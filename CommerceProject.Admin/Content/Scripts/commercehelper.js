function showLoadingModal() {
    $('body').loadingModal({ text: 'Lütfen Bekleyin...' });
    $('body').loadingModal('animation', 'cubeGrid');
}

function hideLoadingModal() {
    $('body').loadingModal('destroy');
}

function callAlert(type, text) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "2000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    toastr[type](text);
}

function validationInvalidHandler(event, validator) {
    var errors = validator.numberOfInvalids();

    $('.scroll-to-top').click();
    $('#form-error-container span').text('Sayfadaki ' + errors + ' kayıt alanını kontrol ediniz.');
    $('#form-error-container').show();
    setTimeout(function () {
        $('#form-error-container').hide(1000);
    }, 7000);
}

function confirm(title, bodyHTML, func) {
    $('#modalConfirm .confirmTitle').text(title);
    $('#modalConfirm .confirmBody').html('');
    $('#modalConfirm .confirmBody').append(bodyHTML);

    $('#modalConfirm .btnYes').bind('click', function () {
        func();

        $('#modalConfirm').modal('hide');
    });

    $('#modalConfirm').modal({ backdrop: 'static' });
    $('#modalConfirm').modal('show');
}

// Confirm Modal (YES?/NO?) kapandıktan sonra unbind events
$('#modalConfirm').on('hidden.bs.modal', function () {
    $('#modalConfirm .btnYes').unbind('click');
});

// Jquery validation x>0 rule
jQuery.validator.addMethod("greaterThanZero", function (value, element) {
    return this.optional(element) || (parseFloat(value) >= 0);
}, "0 veya 0' dan büyük bir değer giriniz.");

// Jquery validation x>0 rule
jQuery.validator.addMethod("tcKimlikNo", function (value, element) {
    value = value.toString();
    var isEleven = /^[0-9]{11}$/.test(value);
    var totalX = 0;
    for (var i = 0; i < 10; i++) {
        totalX += Number(value.substr(i, 1));
    }
    var isRuleX = totalX % 10 == value.substr(10, 1);
    var totalY1 = 0;
    var totalY2 = 0;
    for (var i = 0; i < 10; i += 2) {
        totalY1 += Number(value.substr(i, 1));
    }
    for (var i = 1; i < 10; i += 2) {
        totalY2 += Number(value.substr(i, 1));
    }
    var isRuleY = ((totalY1 * 7) - totalY2) % 10 == value.substr(9, 0);
    return this.optional(element) || (isEleven && isRuleX && isRuleY);

    //return this.optional(element) || (parseFloat(value) > 0);
}, "Geçerli T.C. kimlik no giriniz.");