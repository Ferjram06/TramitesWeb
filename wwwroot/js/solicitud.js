
document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("solicitudForm");
    if (!form) return;

    const errorBox = document.getElementById("formError");
    const modalElement = document.getElementById("folioModal");
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    
    const correo = document.getElementById("correo");
    const telefono = document.getElementById("telefono");

    function validarCorreo() {
        const valor = correo.value.trim();
        const partes = valor.split("@");

        let valido = false;

        if (partes.length === 2) {
            const [usuario, dominio] = partes;
            const etiquetas = dominio.split(".");
            const nombreDominio = etiquetas.slice(0, -1);

            const usuarioValido =
                usuario.length > 0 &&
                usuario.length <= 64 &&
                !usuario.startsWith(".") &&
                !usuario.endsWith(".") &&
                !usuario.includes("..") &&
                /^[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+$/.test(usuario);

            const dominioValido =
                etiquetas.length >= 2 &&
                etiquetas.every(etiqueta =>
                    /^[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?$/.test(etiqueta)
                ) &&
                /^[A-Za-z]{2,63}$/.test(etiquetas.at(-1)) &&
                nombreDominio.some(etiqueta => /[A-Za-z]/.test(etiqueta));

            valido = usuarioValido && dominioValido;
        }

        correo.setCustomValidity(
            valido ? "" : "Introduce un correo válido, como usuario@gmail.com."
        );

        return valido;
    }

    function validarTelefono() {
        const valor = telefono.value.trim();
        let digitos = valor.replace(/\D/g, "");

        // Acepta el prefijo internacional mexicano +52.
        if (valor.startsWith("+52") && digitos.startsWith("52")) {
            digitos = digitos.slice(2);
        }

        const valido = /^\d{10}$/.test(digitos);

        telefono.setCustomValidity(
            valido
                ? ""
                : "Introduce 10 dígitos mexicanos, incluyendo la LADA. Puedes usar +52."
        );

        return valido;
    }


    const maxSize = 10 * 1024 * 1024;

    const files = [
        {
            input: document.getElementById("archivoPdf"),
            status: document.getElementById("pdfStatus"),
            extension: ".pdf",
            label: "PDF"
        },
        {
            input: document.getElementById("archivoTxt"),
            status: document.getElementById("txtStatus"),
            extension: ".txt",
            label: "TXT"
        }
    ];

    function updateFileStatus(item) {
        const file = item.input.files[0];

        item.input.setCustomValidity("");

        if (!file) {
            item.status.className = "file-status mt-2";
            item.status.textContent = "○ Pendiente de selección";
            return false;
        }

        const validExtension = file.name.toLowerCase().endsWith(item.extension);
        const validSize = file.size > 0 && file.size <= maxSize;

        if (!validExtension || !validSize) {
            const message = !validExtension
                ? `Selecciona un archivo ${item.label} válido por extensión.`
                : "El archivo debe tener un tamaño mayor que cero y no superar 10 MB.";

            item.input.setCustomValidity(message);
            item.status.className = "file-status mt-2 field-error";
            item.status.textContent = `✗ ${message}`;
            return false;
        }

        item.status.className = "file-status mt-2 field-success";
        item.status.textContent = `✓ ${item.label} seleccionado: ${file.name}`;
        return true;
    }

    files.forEach(item => {
        item.input.addEventListener("change", () => {
            updateFileStatus(item);
            errorBox.classList.add("d-none");
        });
    });

    form.querySelectorAll("input, select").forEach(field => {
        field.addEventListener("input", () => {
            field.classList.remove("is-invalid");
            if (field === correo) validarCorreo();
            if (field === telefono) validarTelefono();
            errorBox.classList.add("d-none");
        });

        field.addEventListener("change", () => {
            field.classList.remove("is-invalid");
            errorBox.classList.add("d-none");
        });
    });

    form.addEventListener("submit", event => {
        event.preventDefault();
 
        validarCorreo();
        validarTelefono();

        const filesValid = files.map(updateFileStatus).every(Boolean);
        const fieldsValid = form.checkValidity();

        form.querySelectorAll("input, select").forEach(field => {
            field.classList.toggle(
                "is-invalid",
                !field.checkValidity()
            );
        });

        if (!fieldsValid || !filesValid) {
            errorBox.textContent =
                "Revisa los campos señalados y adjunta ambos documentos correctamente.";
            errorBox.classList.remove("d-none");
            errorBox.focus();

            const firstInvalid = form.querySelector(":invalid");
            firstInvalid?.focus();
            return;
        }

        // Folio exclusivamente demostrativo; no garantiza unicidad.
        const number = Math.floor(Math.random() * 99999) + 1;
        document.getElementById("folioNumber").textContent =
            `TR-${String(number).padStart(5, "0")}`;

        modal.show();
    });

    form.addEventListener("reset", () => {
        setTimeout(() => {
            errorBox.classList.add("d-none");

            form.querySelectorAll(".is-invalid").forEach(field => {
                field.classList.remove("is-invalid");
            });

            files.forEach(item => updateFileStatus(item));
        }, 0);
    });
});
