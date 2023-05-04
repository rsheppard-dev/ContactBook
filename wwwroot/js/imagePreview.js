// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const contactImage = document.getElementById("contactIamge");
const imageInput = document.getElementById("imageInput");

function previewImage() {
    contactImage.src = window.URL.createObjectURL(this.files[0]);
}

imageInput.addEventListener('change', previewImage)