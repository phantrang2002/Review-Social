﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    const textEditor = document.getElementById("text-editor");
    const imageInput = document.getElementById("image-file");

    // Image insertion function
    async function insertImage(file) {
        while (textEditor.firstChild) {
            textEditor.removeChild(textEditor.firstChild);
        }
        for (let i of file) {
            const reader = new FileReader();
            await reader.readAsDataURL(i);
            reader.onload = async function () {
                const image = new Image();
                image.src = await reader.result;
                await textEditor.appendChild(image);
            };
        }
    }

    CKEDITOR.replace("content-home");
    CKEDITOR.replace("content-homes");

    // Listen for image selection and insert into text editor
    if (imageInput) {
        imageInput.addEventListener("change", async function (e) {
            const file = e.target.files;
            insertImage(file);
        });
    }
});


// gốc
// const textEditor = document.getElementById("text-editor");
// const imageInput = document.getElementById("image-file");

// async function insertImage(file) {
//     while (textEditor.firstChild) {
//         textEditor.removeChild(textEditor.firstChild);
//     }
//     for (let i of file) {
//         const reader = new FileReader();
//         await reader.readAsDataURL(i);
//         reader.onload = async function () {
//             const image = new Image();
//             image.src = await reader.result;
//             await textEditor.appendChild(image);
//         };
//     }
// }

// CKEDITOR.replace("content-home");
// CKEDITOR.replace("content-homes");


// imageInput.addEventListener("change", async function (e) {
//     const file = e.target.files;
//     insertImage(file);
// });



// của mình
// document.addEventListener("DOMContentLoaded", function () {
//     const textEditor = document.getElementById("text-editor");
//     const imageInput = document.getElementById("image-file");

//     // Image insertion function
//     async function insertImage(file) {
//         while (textEditor.firstChild) {
//             textEditor.removeChild(textEditor.firstChild);
//         }
//         for (let i of file) {
//             const reader = new FileReader();
//             await reader.readAsDataURL(i);
//             reader.onload = async function () {
//                 const image = new Image();
//                 image.src = await reader.result;
//                 await textEditor.appendChild(image);
//             };
//         }
//     }

//     CKEDITOR.replace("content-home");
//     CKEDITOR.replace("content-homes");

//     // Listen for image selection and insert into text editor
//     if (imageInput) {
//         imageInput.addEventListener("change", async function (e) {
//             const file = e.target.files;
//             insertImage(file);
//         });
//     }
// });



