$(".header-slick-slider").slick({
    infinite: true,
    slidesToShow: 4,
    slidesToScroll: 1,
    autoplay: true,
    arrows: false,
   /* prevArrow: `<button type='button' class='slick-prev pull-left'><img src="./images/arrow-left.svg"/></button>`,
    nextArrow: `<button type='button' class='slick-next pull-right'><img src="./images/arrow-right.svg"/></button>`,*/
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1,
                infinite: true,
                arrows: false,
            },
        },
        {
            breakpoint: 480,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1,
                infinite: true,
                arrows: false,
            },
        },
    ],
});

$('body').on("change", "#upload-photo", function () {

    $this = $(this);
    $form = $this.parents("form");

    if (window.FormData !== undefined) {

        var fileUpload = $("#upload-photo").get(0);
        var files = fileUpload.files;

        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        $.ajax({
            url: '/Comestic/UploadFiles',
            type: "POST",
            contentType: false,
            processData: false,
            data: fileData,
            success: function (result) {
                var fileImage = "";
                if (result.result) {
                    fileImage = result.filename;
                }
                console.log(fileImage);
                $("#objectPhoto").val(fileImage);
                $("#id").val('');
                document.querySelector("form[action='/Comestic']").submit()
            },
            error: function (err) {
                console.log(err.statusText);
            }
        });
    } else {
        console.log("FormData is not supported.");
    }
});

