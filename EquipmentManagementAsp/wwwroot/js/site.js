// Função para mostrar os toasts
function showToasts() {
    var toastElements = document.querySelectorAll('.toast');
    toastElements.forEach(function (toastEl) {
        var toast = new bootstrap.Toast(toastEl, {
            delay: 3000, // Define o tempo para o toast sumir após 3 segundos (3000ms)
            autohide: true // Garante que o toast desapareça automaticamente
        });
        toast.show(); // Exibe o toast
    });
}

// Adiciona o código para ser executado somente quando a página carregar
function initializeToastMessages() {
    document.addEventListener("DOMContentLoaded", function () {
        showToasts();
    });
}