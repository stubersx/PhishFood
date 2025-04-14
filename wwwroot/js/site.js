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

function showModal() {
    modal = document.getElementsByClassName("modal");
    modal[index - 2].style.display = "block";
}

function closeModal() {
    for (i = 0; i < modal.length; i++) {
        modal[i].style.display = "none";
    }
}
