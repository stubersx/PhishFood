/* dropdown */
function Dropdown() {
    document.getElementById("dropdown-content").classList.toggle("show");
}

window.onclick = function (event) {
    if (!event.target.matches('.dropbtn')) {
        let dropdowns = document.getElementsByClassName("dropdown-content");
        for (let i = 0; i < dropdowns.length; i++) {
            if (dropdowns[i].classList.contains('show')) {
                dropdowns[i].classList.remove('show');
            }
        }
    }
}

/* slideshow */
let index = 1;
showSlides(index);

function plusSlides(n) {
    showSlides(index += n);
}

function currentSlide(n) {
    showSlides(index = n);
}

function showSlides(n) {
    let i;
    let slides = document.getElementsByClassName("slides");
    let dots = document.getElementsByClassName("dot");

    if (n >= slides.length)
        document.getElementsByClassName("next")[0].style.visibility = "hidden";
    else
        document.getElementsByClassName("next")[0].style.visibility = "visible";
    if (n <= 1)
        document.getElementsByClassName("prev")[0].style.visibility = "hidden";
    else
        document.getElementsByClassName("prev")[0].style.visibility = "visible";
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[index - 1].style.display = "block";
    dots[index - 1].className += " active";
}

/* modal */
let modal;

function showModal(i) {
    closeModal();  // Hide all modals before opening a new one
    let modal = document.getElementById(`modal-${i}`);
    if (modal) {
        modal.style.display = "flex";
    }
    toggleNavButtons(false);
}

function closeModal() {
    let modals = document.getElementsByClassName("modal");
    for (let i = 0; i < modals.length; i++) {
        modals[i].style.display = "none";
    }
    toggleNavButtons(true);
}

function toggleNavButtons(show) {
    const display = show ? "block" : "none";
    const prev = document.querySelector(".prev");
    const next = document.querySelector(".next");

    if (prev) prev.style.display = display;
    if (next) next.style.display = display;
}

document.querySelectorAll('.modal').forEach(modal => {
    modal.addEventListener('click', function (event) {
        // If the click target is the modal itself (background), close it
        if (event.target === modal) {
            modal.style.display = 'none';
        }
        toggleNavButtons(true);
    });
});
