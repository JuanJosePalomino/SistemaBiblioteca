$(document).on('click', '#searchButton', function (e) {
    searchMaterialRequest(e);
});

$(document).on('click', '#addMaterialToReserve', function (e) {
    addMaterialToLocalStorage(e);
});

$(document).on('click', '.show-material-info', function (e) {
    showMaterialInfo(e);
});

$(document).on('click', '#completeReservation', function (e) {
    completarReserva(e);
});

$(document).on('click', '#showReservationMaterial', function (e) {
    reservationMaterial();
});

$(document).on('click', '.delete-material', function (e) {
    deleteMaterial(e);
});

$(document).on('click', '#reservar', function (e) {
    if ($('#isValidUser').val() == "isValid") {
        
        if ($("#daysFormInfo").val() != 0) {
            $('#modalConfirmation').modal('show');
        } else {
            $('.fields-container').append('<p class="validation-message col-md-6" style="color:red;">Los días de reserva tienen que ser mayores a 0</p>');
            setTimeout(function (e) {
                $('.validation-message').remove();
            }, 6000);
        }
    } else {
        $('.fields-container').append('<p class="validation-message col-md-6" style="color:red;">Debes realizar el paso de validar el usuario y cumplir con los requisitos para reservar</p>');
        setTimeout(function (e) {
            $('.validation-message').remove();
        }, 6000)
    }
});

$(document).on('click', '#validar', function (e) {
    validarUsuario(e);
});


$(window).on("load", function (e) {
    fillFormFields();
    $('.quantity-material-reservation').text(window.localStorage.getItem("material") ? JSON.parse(window.localStorage.getItem("material")).length : 0);
});


function fillFormFields() {
    if (window.localStorage.getItem("material")) {
        let dataLocalStorage = JSON.parse(window.localStorage.getItem("material"));
        let endDates = dataLocalStorage.map(x => x.endDate);
        let startDates = dataLocalStorage.map(x => x.startDate);

        let sortedEndDates = endDates.sort((a, b) => new Date(b).getTime() - new Date(a).getTime());

        let sortedStartDates = startDates.sort((a, b) => new Date(b).getTime() - new Date(a).getTime());

        let startDateReserve = sortedStartDates.slice(-1)[0];
        let endDateReserve = sortedEndDates[0];

        let timeEndDate = new Date(endDateReserve).getTime();
        let timeStartDate = new Date(startDateReserve).getTime();

        let days = Math.ceil((parseInt(timeEndDate) - parseInt(timeStartDate)) / (1000 * 3600 * 24));

        $('#startDateFormInfo').val(startDateReserve);
        $('#endDateFormInfo').val(endDateReserve);
        $('#daysFormInfo').val(days);
    }
}

function searchMaterialRequest(){
    $.ajax({
        url: window.location.origin + "/Material/ConsultarMaterial",
        method: "GET",
        data: {
            parameter: $('#searchInput').val()
        }
    }).done(function (result) {
        let materiales = result;
        $('#materialListContainer').find('.material-row').remove();
        if (materiales.length > 0 && $('#materialListContainer').find('#materialList')) {
            $('.no-encontrado').remove();
            $(materiales).each(function () {
                let clonedMaterialBase = $("#materialToClone").clone();
                $(clonedMaterialBase).css('display', 'block');
                $(clonedMaterialBase).removeAttr('hidden');
                $(clonedMaterialBase).removeAttr('id');
                $(clonedMaterialBase).find('.titulo').text(this.titulo);
                $(clonedMaterialBase).find('.categoria').text(this.categoria);
                $(clonedMaterialBase).find('.paginas').text(this.paginas);
                $('#materialListContainer').find('#materialList').append(clonedMaterialBase);
            });
        } else {
            $('#materialListContainer').append('<div class="d-flex justify-content-center mt-5 no-encontrado"><h3>No se encontraron resultados</h3></div>')
        }
    });
}

function showMaterialInfo(e) {
    let materialId = $(e.currentTarget).data('itemid');
    window.location.href = window.location.origin + "/Reserva/InformacionMaterial?id=" + materialId;
}

function addMaterialToLocalStorage(e) {
    let materialId = $(e.currentTarget).data('itemid');
    let startDate = $('#startDate').val();
    let endDate = $('#endDate').val();
    let materialToAdd = {
        materialId: materialId,
        startDate: startDate,
        endDate: endDate
    }
    let currentMaterialAdded = null;
    if (window.localStorage.getItem("material")) {
        currentMaterialAdded = JSON.parse(window.localStorage.getItem("material"));
        let flag = true;
        $(currentMaterialAdded).each(function () {
            if (materialToAdd.materialId == this.materialId) {
                flag = false;
                window.history.go(-1);
                return;
            }
        });
        if (flag) {
            currentMaterialAdded.push(materialToAdd);
            window.localStorage.setItem("material", JSON.stringify(currentMaterialAdded));
        }
    } else {
        let materialIdList = JSON.stringify([materialToAdd]);
        window.localStorage.setItem("material", materialIdList);
    }
    
}

function validarUsuario(e) {
    $.ajax({
        url: window.location.origin + "/Usuario/ValidateUser",
        method: "GET",
        data: {
            id: $('#userId').val() != null ? $('#userId').val() : " "
        }
    }).done(function (result) {
        if (result.isValid) {
            $('.fields-container').append('<p class="validation-message col-md-6" style="color:green;">El usuario cumple con los requisitos para reservar</p>');
            $('.fields-container').append('<input type="hidden" value="isValid" id="isValidUser">');
        } else {
            $('.fields-container').append('<p class="validation-message col-md-6" style="color:red;">El usuario no cumple con los requisitos para reservar</p>');
        }

        setTimeout(function () {
            $('.validation-message').remove();
        }, 7000);
    });
}

function completarReserva() {

    let materialIds = JSON.parse(window.localStorage.getItem("material")).map(x => x.materialId);
    let htmlFactura = "<h4>Reserva realizada</h4><div><p><strong>Fecha inicio: </strong> " + $('#startDateFormInfo').val() + " </p><p><strong>Fecha fin: </strong> " + $('#endDateFormInfo').val() + " </p><p><strong>Total días: </strong> " + $('#daysFormInfo').val() + " </p><p><strong>Cantidad material: </strong> " + materialIds.length + " </p></div>"

    let data = {
        FechaInicio: $('#startDateFormInfo').val(),
        FechaFin: $('#endDateFormInfo').val(),
        DiasReserva: $('#daysFormInfo').val(),
        MaterialIds: materialIds,
        IdUsuario: 1,
        HTMLFactura: htmlFactura

    }

    $.ajax({
        url: window.location.origin + "/Reserva/CompletarReserva",
        method: "POST",
        data: {
            request: JSON.stringify(data)
        }
    }).done(function (result) {
        window.location.href = window.location.origin + result.redirectUrl;
        window.localStorage.clear();
        setTimeout(function () {
            window.location.href = window.location.origin;
        }, 10000);
    });
}

function reservationMaterial() {
    let materialIds = JSON.parse(window.localStorage.getItem("material")).map(x => x.materialId);
    window.location.href = window.location.origin + "/Reserva/ReservaFin?request=" + JSON.stringify(materialIds);
}

function deleteMaterial(e) {
    $(e.currentTarget).parents('.material-row').remove();
    
    if (window.localStorage.getItem("material")) {
        let currentMaterialsToReservation = JSON.parse(window.localStorage.getItem("material"));
        
        $(currentMaterialsToReservation).each(function (index) {
            if (this.materialId == $(e.currentTarget).data('itemid')) {
                currentMaterialsToReservation.splice(index, 1);
            }
        });

        reservationMaterial();

    }
}