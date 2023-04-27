async function EditorUploadImageToOss(editorContentElement, editorElement, ossParamter) {
    var fileInputElement = editorElement.querySelector('input.ql-image[type=file]')
    var imageFiles = await UploadImage(fileInputElement, ossParamter);

    let quill = editorContentElement.__quill;//get quill editor
    for (var i = 0; i < imageFiles.length; i++) {
        let length = quill.getSelection().index;
        quill.insertEmbed(length, 'image', imageFiles[i]);//Insert the uploaded picture into the editor
        quill.setSelection(length + 1);
    }
}

async function UploadImage(element, ossParamter) {
    var imageFiles = element.files ?? element;
    console.log("UploadImage", ossParamter);
    const client = new OSS(ossParamter);

    const headers = {
        'x-oss-object-acl': 'public-read-write',
    };

    var result = [];
    for (var i = 0; i < imageFiles.length; i++) {
        var url = await putObject(client, imageFiles[i], headers);
        result.push(url);
    }

    return result
}

async function putObject(client, file, headers) {
    try {
        const result = await client.put(
            `${file.name}`,
            file,
            {
                headers
            }
        );
        console.log(result);
        return result.url;
    } catch (e) {
        console.log(e);
    }
}