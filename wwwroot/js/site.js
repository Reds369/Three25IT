// Three25IT client-side helpers

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        // Make whole table rows navigate when they contain a row-link target.
        document.querySelectorAll('tr.row-link').forEach(function (row) {
            row.addEventListener('click', function (event) {
                if (event.target.closest('a, button, input, select, label')) return;
                var link = row.querySelector('a');
                if (link) window.location.href = link.href;
            });
        });

        // Auto-dismiss success alerts after a delay.
        var successAlerts = document.querySelectorAll('.alert-success.alert-dismissible');
        successAlerts.forEach(function (alert) {
            window.setTimeout(function () {
                var instance = bootstrap.Alert.getOrCreateInstance(alert);
                if (instance) instance.close();
            }, 8000);
        });

        // Warn before leaving the payment page with an unsubmitted payment in progress.
        var paymentForm = document.querySelector('form[action$="/Billing/Pay"]');
        if (paymentForm) {
            paymentForm.addEventListener('submit', function () {
                paymentForm.dataset.submitted = 'true';
            });
            window.addEventListener('beforeunload', function (event) {
                if (paymentForm.dataset.submitted === 'true') return;
                event.preventDefault();
                event.returnValue = '';
            });
        }

        // Keep the selected invoice's amount in sync on the payment screen.
        var invoiceSelect = document.getElementById('InvoiceId');
        if (invoiceSelect) {
            invoiceSelect.addEventListener('change', function () {
                var text = invoiceSelect.options[invoiceSelect.selectedIndex].text;
                var match = text.match(/R([\d,]+\.\d{2})/);
                var button = paymentForm ? paymentForm.querySelector('button[type="submit"]') : null;
                if (button && match) {
                    button.innerHTML = '<i class="bi bi-lock me-2"></i>Pay R' + match[1];
                }
            });
        }
    });
})();
