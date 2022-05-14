//block the control key
onkeydown = e => {
    let tecla = e.which || e.keyCode;

    // Evaluar si se ha presionado la tecla Ctrl:
    if (e.ctrlKey) {
        // Evitar el comportamiento por defecto del nevagador:
        e.preventDefault();
        e.stopPropagation();
    }
}