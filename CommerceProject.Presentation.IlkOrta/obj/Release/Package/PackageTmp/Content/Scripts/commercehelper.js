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
//jQuery.validator.addMethod("greaterThanZero", function (value, element) {
//    return this.optional(element) || (parseFloat(value) > 0);
//}, "0' dan büyük bir değer giriniz.");