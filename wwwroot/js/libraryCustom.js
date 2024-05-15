//tắt tạm
function ajaxCustom(url, method, data, successCallback, errorCallback) {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function() {
      if (xmlhttp.readyState == 4) {
        if (xmlhttp.status == 200) {
          successCallback(xmlhttp.responseText);
        } else {
          errorCallback(xmlhttp.statusText);
        }
      }
    };
    xmlhttp.open(method, url, true);
    xmlhttp.send(data);
}
// cach su dung ->
// ajaxCustom(url, method, data, function(response) {
//     // xử lý kết quả thành công
//   }, function(error) {
//     // xử lý khi có lỗi xảy ra
//   });

const imagePreview = document.getElementById("imagePreview");
const imageIn = document.getElementById("file");
async function preview(file) {
    while (imagePreview?.firstChild) {
        imagePreview.removeChild(imagePreview.firstChild);
    }
    for (let i of file) {
        const reader = new FileReader();
        await reader.readAsDataURL(i);
        reader.onload = async function () {
            const image = new Image();
            image.src = await reader.result;
            await imagePreview.appendChild(image);
        };
    }
}
imageIn.addEventListener("change", async function (e) {
    const file = e.target.files;
    preview(file);
}
  
)
const imagePreview2 = document.getElementById("imagePreview2");
const imageIn2 = document.getElementById("file2");
async function preview2(file) {
    while (imagePreview2?.firstChild) {
        imagePreview2.removeChild(imagePreview2.firstChild);
    }
    for (let i of file) {
        const reader = new FileReader();
        await reader.readAsDataURL(i);
        reader.onload = async function () {
            const image = new Image();
            image.src = await reader.result;
            await imagePreview2.appendChild(image);
        };
    }
}
imageIn2?.addEventListener("change", async function (e) {
    const file = e.target.files;
    preview2(file);
}
)


const updateField = async (id,title,content,categoryId,thumb) =>{
    console.log(id,title,content,categoryId,thumb)
     var editor = CKEDITOR.instances['content-homes'];
    const titlechange = document.getElementById("title2");
    titlechange.value= title || ''
    const option = document.getElementById("imagePreview2")
    const select = document.getElementById("exampleFormControlSelect2");
   
    const btn = document.getElementById("btnUpdate");
    btn.addEventListener("click",()=> updatePost(id))

    select.childNodes.forEach(e =>{
        console.log(e.value,e.selected)
        if(e.value == categoryId)
        {
            e.selected=true
        }
    })
    content &&  await  editor.setData(content)
    
    option.children.length > 0 && option.removeChild(option.firstChild);
     
    if(thumb){
        const image = new Image();
    image.src = thumb;
   await option.appendChild(image);
    }

}
const deleteItem = (itemId) => {
    
    if (confirm("Bạn có muốn xóa bài viết này?")) {
 
    ajaxCustom(`/Home/Delete/${itemId}`,'delete',null,(value)=>{window.location.reload()},(value)=>{
        alert('xóa không thành công')
    })
 
        }


 };
  const updatePost = (id) => {
    var editor = CKEDITOR.instances['content-homes'];
    const title = document.getElementById("title2").value;
    const option = document.getElementById("exampleFormControlSelect2").value;
    const file = document.getElementById("file2").files[0]

    var formData = new FormData();
    formData.append("Id",id);
    formData.append('file', file);
    formData.append('Title', title);
    formData.append('Content', editor.getData());
    formData.append('CategoryId', option);
    ajaxCustom("Home/UpdateCmt","post",formData,(value)=>{window.location.reload()},(value)=>{alert('cập nhật không thành công')})
}

//tắt tạm
const submitForm = () => {
    var editor = CKEDITOR.instances['content-home'];
    const title = document.getElementById("title").value;
    const verifykey = document.getElementById("verifykey").value;
    const option = document.getElementById("exampleFormControlSelect1").value;
    const file = document.getElementById("file").files[0]

    var formData = new FormData();
    //console.log(value);

    formData.append('file', file);
    formData.append('Title', title);
    formData.append('verifykey', verifykey);
    formData.append('Content', editor.getData());
    formData.append('CategoryId', option);
    ajaxCustom("Home/Create","post",formData,(value)=>{window.location.reload()},(value)=>{alert('thêm không thành công')})
}



//tắt tạm - chatgpt - k biết hđ k
// function ajaxCustom(url, method, data, successCallback, errorCallback) {
//     var xmlhttp = new XMLHttpRequest();
//     xmlhttp.onreadystatechange = function() {
//         if (xmlhttp.readyState == 4) {
//             if (xmlhttp.status == 200) {
//                 successCallback(xmlhttp.responseText);
//             } else {
//                 errorCallback(xmlhttp.statusText);
//             }
//         }
//     };
//     xmlhttp.open(method, url, true);

//     // Kiểm tra xem method là POST thì thêm dữ liệu vào request
//     if (method.toUpperCase() === "POST") {
//         xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
//         console.log(data);
//         xmlhttp.send(data); // Gửi dữ liệu dưới dạng chuỗi truy vấn URL
//     } else {
//         xmlhttp.send();
//     }
// }

// const submitForm = () => {
//     console.log(122);
//     var editor = CKEDITOR.instances['content-home'];
//     const title = document.getElementById("title").value;
//     const option = document.getElementById("exampleFormControlSelect1").value;
//     const file = document.getElementById("file").files[0];
//     console.log(file);
//     var formData = new FormData();
//     formData.append('file', file);
//     formData.append('Title', title);
//     formData.append('Content', editor.getData());
//     formData.append('CategoryId', option);



//     // Chuyển đổi FormData thành chuỗi truy vấn URL
//     const formDataString = new URLSearchParams(formData).toString();
//     ajaxCustom("Home/Create", "post", formDataString, (value) => {
//         window.location.reload();
//     }, (value) => {
//         alert('thêm không thành côngggg');
//     });
// }





  



