
document.addEventListener("DOMContentLoaded", () => {
    const button = document.getElementById("downloadReport");
    const loader = document.getElementById("reportLoader");
    const status = document.getElementById("reportStatus");

    if (!button || !loader || !status) return;

    button.addEventListener("click", () => {
        button.disabled = true;
        status.className = "alert d-none";
        loader.classList.remove("d-none");

        // Simulación temporal del procesamiento.
        window.setTimeout(() => {
            loader.classList.add("d-none");
            status.className = "alert alert-success";
            status.textContent =
                "✓ Reporte generado correctamente. Esta descarga es simulada; todavía no se creó ningún archivo.";
            button.disabled = false;
            status.focus();
        }, 1500);
    });
});
