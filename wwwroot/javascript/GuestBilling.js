/* Guest Billing Js functions*/

/*$(document).ready(function () {
    var checkInId = $('#hiddenCheckInId').val();
    if (checkInId != '' && checkInId != 0 && checkInId != null) {
        loadBill();
    }
});*/


// Bill

function addNewBill() {

    var checkInId = $('#hiddenCheckInId').val();

    const columnStyles = {
        padding: '12px',
        width: '100%',
        border: 'none',
        color: '#31849B',
        textAlign: 'center'
    };

    const leftColumnStyles = {
        ...columnStyles,
        textAlign: 'left'
    }


    const newRow = $('<tr>').css({ 'text-align': 'left' });

    // select serviceDesc
    var selectSrvcCde = $('<select>').css(leftColumnStyles);
    $.ajax({
        url: "/GuestBilling/GetSrvcCodes",
        success: function (srvcCdes) {
            $("<option>").val('').text("Select One").appendTo(selectSrvcCde);
            srvcCdes.forEach(srvcCde => {
                $("<option>").val(srvcCde.srvccde).text(srvcCde.srvcdesc).appendTo(selectSrvcCde);
            });
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
            return;
        }
    });
    newRow.append($('<td>').css('padding', '0px').append(selectSrvcCde));

    selectSrvcCde.on('change', function () {
        var inputData = {
            checkInId: checkInId,
            srvcCde: this.value
        }
        $.ajax({
            url: "/GuestBilling/GetOthersBySrvcCde",
            data: inputData,
            success: function (other) {
                inputSrvcCde.val(other.srvcCde);
                selectDeptCde.val(other.deptCde);
                selectFolio.val(other.folioCde);
            },
            error: function () {
                alert('fail to get by data.');
            }
        });
    });

    // input service code
    var inputSrvcCde = $('<input>').css(leftColumnStyles).attr({ 'readonly': 'readonly' }).val('').css('color', 'black');
    newRow.append($('<td>').css('padding', '0px').append(inputSrvcCde));

    // select departments
    var selectDeptCde = $('<select>').css(leftColumnStyles);
    $.ajax({
        url: "/GuestBilling/GetDepartments",
        success: function (departments) {
            departments.forEach(department => {
                $("<option>").val(department.deptcde).text(department.deptcde).appendTo(selectDeptCde);
            });
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
            return;
        }
    });
    newRow.append($('<td>').css('padding', '0px').append(selectDeptCde));

    // select folios
    var selectFolio = $('<select>').css(leftColumnStyles);
    $.ajax({
        type: "GET",
        url: "/GuestBilling/GetFoliosById",
        data: { checkInId: checkInId },
        success: function (folios) {
            console.log(folios);
            $("<option>").val('MF').text('MF').appendTo(selectFolio);
            folios.forEach(folio => {
                $("<option>").val(folio.folioCde).text(folio.folioCde).appendTo(selectFolio);
            });
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
            return;
        }
    });
    newRow.append($('<td>').css('padding', '0px').append(selectFolio));

    // input Qty
    var inputQty = $('<input>').css(columnStyles).attr('type', 'number').val(1);
    newRow.append($('<td>').css('padding', '0px').append(inputQty));

    inputQty.on('input', function () {
        var qty = inputQty.val();
        if (qty == '' || qty == 'undefined') {
            qty = 0;
        }
        var totalAmt = parseFloat(qty) * parseFloat(inputPrice.val());
        inputAmount.val(totalAmt);
    })

    // input Price
    var inputPrice = $('<input>').css(leftColumnStyles).attr('type', 'number').val(0);
    newRow.append($('<td>').css('padding', '0px').append(inputPrice));
    inputPrice.on('input', function () {
        var price = inputPrice.val();
        if (price == '' || price == 'undefined') {
            price = 0;
        }
        var totalAmt = parseFloat(inputQty.val()) * parseFloat(price);
        inputAmount.val(totalAmt);
    })

    // input Amount
    var inputAmount = $('<input>').css(leftColumnStyles).attr({ 'readonly': 'readonly' }).val(0).css('color', 'black');
    newRow.append($('<td>').css('padding', '0px').append(inputAmount));

    // input Remark
    var inputRemark = $('<input>').css(leftColumnStyles);
    newRow.append($('<td>').css('padding', '0px').append(inputRemark));

    // td Delete
    var iDel = $('<i>').addClass('fas fa-times').css({
        'color': 'red',
        'padding': '0px'
    });
    var tdDel = $('<td>').css('cursor', 'pointer').append(iDel);
    newRow.append(tdDel);
    tdDel.on('click', function () {
        newRow.remove();
    });

    newRow.on('keypress', function (event) {
        if (event.keyCode === 13) {
            addNewBill();
        }
    })

    $('#addBillTableBody').append(newRow);

}

function calculateAmt(qty, price) {
    var totalAmt = qty * price;
    inputAmount.val(totalAmt);
}

function loadBill() {

    $('#loadingLabel').show();

    var checkInId = $('#hiddenCheckInId').val();

    var inputData = {
        checkInId: checkInId
    };

    const columnStyles = {
        padding: '12px',
        width: '100%',
        border: 'none',
        color: '#31849B',
        textAlign: 'center'
    };

    const leftColumnStyles = {
        ...columnStyles,
        textAlign: 'left'
    }

    const rightColumnStyles = {
        ...columnStyles,
        textAlign: 'right'
    }

    $.ajax({
        type: 'GET',
        url: '/GuestBilling/LoadBills',
        data: inputData,
        success: function (bills) {
            $('#SrvcFolioTableBody').empty();
            bills.forEach(function (bill) {
                const newRow = $('<tr>').css({ 'text-align': 'left' });

                // select serviceCode
                var selectSrvcCde = $('<select>').css(leftColumnStyles);
                $.ajax({
                    url: "/GuestBilling/GetSrvcCodes",
                    success: function (srvcCdes) {
                        $("<option>").val('').text("Select One").appendTo(selectSrvcCde);
                        srvcCdes.forEach(srvcCde => {
                            $("<option>").val(srvcCde.srvccde).text(srvcCde.srvccde).appendTo(selectSrvcCde);
                        });
                        selectSrvcCde.val(bill.srvccde);
                    },
                    error: function () {
                        alert('Session Expired!');
                        window.location.href = '/MsUsers/Login';  // Redirect to login
                        return;
                    }
                });
                newRow.append($('<td>').css('padding', '0px').append(selectSrvcCde));

                selectSrvcCde.on('change', function () {
                    var inputData = {
                        checkInId: checkInId,
                        srvcCde: this.value
                    }

                    $.ajax({
                        url: "/GuestBilling/GetOthersBySrvcCde",
                        data: inputData,
                        success: function (other) {
                            selectDeptCde.val(other.deptCde);
                            selectFolio.val(other.folioCde);
                        },
                        error: function () {
                            alert('Session Expired!');
                            window.location.href = '/MsUsers/Login';  // Redirect to login
                        }
                    });
                });

                // select departments
                var selectDeptCde = $('<select>').css(leftColumnStyles);
                $.ajax({
                    url: "/GuestBilling/GetDepartments",
                    success: function (departments) {
                        departments.forEach(department => {
                            $("<option>").val(department.deptcde).text(department.deptcde).appendTo(selectDeptCde);
                        });
                        selectDeptCde.val(bill.deptcde);
                    },
                    error: function () {
                        alert('Session Expired!');
                        window.location.href = '/MsUsers/Login';  // Redirect to login
                        return;
                    }
                });
                newRow.append($('<td>').css('padding', '0px').append(selectDeptCde));

                // select folios
                var selectFolio = $('<select>').css(leftColumnStyles);
                $.ajax({
                    type: "GET",
                    url: "/GuestBilling/GetFoliosById",
                    data: { checkInId: checkInId },
                    success: function (folios) {
                        $("<option>").val('MF').text('MF').appendTo(selectFolio);
                        folios.forEach(folio => {
                            $("<option>").val(folio.folioCde).text(folio.folioCde).appendTo(selectFolio);
                        });
                        selectFolio.val(bill.foliocde);
                    },
                    error: function () {
                        alert('Session Expired!');
                        window.location.href = '/MsUsers/Login';  // Redirect to login
                        return;
                    }
                });
                newRow.append($('<td>').css('padding', '0px').append(selectFolio));

                // input Qty
                var inputQty = $('<input>').css(columnStyles).attr('type', 'number').val(bill.qty);
                newRow.append($('<td>').css('padding', '0px').append(inputQty));

                // input Price
                var inputPrice = $('<input>').css(leftColumnStyles).attr('type', 'number').val(bill.pricebill);
                newRow.append($('<td>').css('padding', '0px').append(inputPrice));

                // input Amount
                var inputAmount = $('<input>').css(leftColumnStyles).attr('type', 'number').val(bill.pricebill);
                newRow.append($('<td>').css('padding', '0px').append(inputAmount));

                // input Remark
                var inputRemark = $('<input>').css(leftColumnStyles).val(bill.remark);
                newRow.append($('<td>').css('padding', '0px').append(inputRemark));

                // td Delete
                var tdDel = $('<td>').css({ 'color': 'red', 'cursor': 'pointer', 'textAlign': 'center' }).text('Remove');
                newRow.append(tdDel);

                tdDel.on('click', function () {
                    newRow.remove();
                });

                $('#addBillTableBody').append(newRow);
            });
            $('#loadingLabel').hide();
        },
        error: function () {
            $('#loadingLabel').hide();
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function saveBill() {

    var checkInId = $('#hiddenCheckInId').val();

    const billTable = document.getElementById('addBillTable');
    const billTableData = [];

    for (let i = 1; i < billTable.rows.length; i++) { // skip Header
        const row = billTable.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            rowData.push(getCellContent(row.cells[j]));
        }
        billTableData.push(rowData);
    }

    var inputData = {
        checkInId: checkInId,
        billTable: billTableData
    }

    $.ajax({
        type: 'POST',
        url: '/GuestBilling/SaveBills',
        data: inputData,
        success: function () {
            alert('Successfully Saved.');
            $('#addBillTableBody').empty();

        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}


function changeAmount(element) {
    var row = element.closest('tr');

    var inputQty = row.querySelector('td:nth-child(5) input').value;
    var inputPrice = row.querySelector('td:nth-child(6) input').value;

    if (inputQty == 'undefined' || inputQty == '' || inputPrice == 'undefined' || inputPrice == '') {
        return;
    }
    var totalAmt = parseFloat(inputQty) * parseFloat(inputPrice);
    var inputAmt = row.querySelector('td:nth-child(7) input');
    inputAmt.value = totalAmt;
}



// Payment

function choosePayment(folioCde, checkInId, totalAmt) {

    if (totalAmt <= 0) {
        alert('Please add bills first for this folio.');
        return;
    }

    $('#paymentModal').show();

    const paymentTypeTable = $('#paymentTypeBodyId');
    const paymentTable = $('#paymentTableBodyId');

    paymentTypeTable.empty(); // empty payment type table data first
    paymentTable.empty(); // empty payment table data

    $.ajax({
        url: "/GuestBilling/GetCurrencies",
        type: "GET",
        success: function (currencies) {
            currencies.forEach(function (currency) {
                const newRow = $('<tr>').attr('id', 'pay-' + currency.currid)
                    .append($('<td>').text(currency.currcde).css('padding', '14px 12px'));
                newRow.on('click', function () {
                    addCurrency(currency.currid, currency.currrate, currency.currcde);
                }).css('cursor', 'pointer');

                paymentTypeTable.append(newRow);
            });

            $('#hiddenCheckInId').val(checkInId);
            $('#spanFolioCde').text(folioCde);
            $('#inputTotalAmt').val(totalAmt);
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function addCurrency(currId, currrate, currtyp) { // Add payment
    const clickedPayType = $('#pay-' + currId); // hiding row after clicked
    clickedPayType.hide();

    const columnStyles = {
        'padding': '14px 12px',
        'cursor': 'pointer',
        'border': 'none',
        'width': '100%',
        'color': '#31849B'
    }

    const newRow = $('<tr>');

    // tdCurrTyp
    var tdPayCurrTyp = $('<td>').text(currtyp).css('padding', '12px');
    newRow.append(tdPayCurrTyp);

    // inputCurrRate
    var inputCurrRate = $('<input>').css(columnStyles).attr({ 'type': 'number' }).val(currrate);

    inputCurrRate.on('input', function () {
        var totalAmt = $('#inputTotalAmt').val();
        var fmtTotalAmt = totalAmt.replace(',', '') / inputCurrRate.val();
        inputPayAmt.val(fmtTotalAmt.toFixed(0));
    });

    newRow.append(($('<td>').css('padding', '0px').append(inputCurrRate)));

    // inputPayAmt
    var totalAmt = $('#inputTotalAmt').val();
    var fmtTotalAmt = totalAmt.replace(',', '') / currrate;
    if (fmtTotalAmt < 0) {
        fmtTotalAmt = -fmtTotalAmt;
    }
    var inputPayAmt = $('<input>').css(columnStyles).attr({ 'type': 'number' }).val(fmtTotalAmt.toFixed(0));
    newRow.append(($('<td>').css('padding', '0px').append(inputPayAmt)));

    // Cancel Btn
    var tdPayCancel = $('<i>').addClass('fas fa-times fa-2x').css({
        'cursor': 'pointer',
        'color': 'red',
        'padding': '12px'
    });
    tdPayCancel.on('click', function () {
        newRow.remove();
        clickedPayType.show();
    });

    newRow.append($('<td>').css('padding', '0px').append(tdPayCancel));

    $('#paymentTableBodyId').append(newRow);
}

function savePayment() {
    var checkInId = $('#hiddenCheckInId').val();
    var folioCde = $('#spanFolioCde').text();
    var srvcCde = $('#inputSrvcCde').val();
    const paymentTable = document.getElementById('paymentTable');
    const paymentTableData = [];

    for (let i = 1; i < paymentTable.rows.length; i++) { // skip Header
        const row = paymentTable.rows[i];
        const rowData = [];
        for (let j = 0; j < row.cells.length; j++) {
            rowData.push(getCellContent(row.cells[j]));
        }
        paymentTableData.push(rowData);
    }

    var inputData = {
        folioCde: folioCde,
        checkInId: checkInId,
        srvcCde: srvcCde,
        paymentTable: paymentTableData
    }

    $.ajax({
        type: 'POST',
        url: '/GuestBilling/SavePayments',
        data: inputData,
        success: function () {
            alert('Successfully Saved.');
            location.reload();
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });
}

function closePaymentModal() {
    $('#paymentModal').hide();
}


// Close Folio

function closeFolio(folioCde, checkInId, totalAmt) {

    if (totalAmt != 0) {
        var userConfirm = confirm('Do you want to forced close ' + folioCde + ' folio?');
        if (userConfirm == false) {
            return;
        }
    }
    else {
        var userConfirm = confirm('Are you sure you want to close ' + folioCde + ' folio?');
        if (userConfirm == false) {
            return;
        }
    }

    var inputData = {
        folioCde: folioCde,
        checkInId: checkInId
    }

    $.ajax({
        url: "/GuestBilling/CloseFolio",
        type: 'POST',
        data: inputData,
        success: function () {
            alert(folioCde + ' Folio is successfully closed.');
            location.reload();
        },
        error: function () {
            alert('Session Expired!');
            window.location.href = '/MsUsers/Login';  // Redirect to login
        }
    });



}


// Void Folio

function voidBill(billId) {
    var userConfirm = confirm('Are you sure you want to void this bill?');
    if (userConfirm) {

        var inputData = {
            billId: billId
        };

        $.ajax({
            url: '/GuestBilling/VoidBill',
            type: 'POST',
            data: inputData,
            success: function () {
                alert('Bill is successfully voided.');
                location.reload();
            },
            error: function () {
                alert('Session Expired!');
                window.location.href = '/MsUsers/Login';  // Redirect to login
            }
        });

    }
}


// Common 

function getCellContent(cell) {
    const select = cell.querySelector("select");
    if (select) {
        return select.value;
    }
    const input = cell.querySelector("input");
    if (input) {
        if (input.type === "checkbox") {
            return input.checked; // Return true or false for checkbox
        } else {
            return input.value;
        }
    }
    return cell.textContent;
}



