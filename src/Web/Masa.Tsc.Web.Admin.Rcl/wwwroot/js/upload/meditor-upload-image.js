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
    var imageFiles = element.files
    console.log("UploadImage", ossParamter);
    const client = new OSS(ossParamter);

    const headers = {
        // Ö¸¶¨¸ÃObject±»ÏÂÔØÊ±ÍøÒ³µÄ»º´æÐÐÎª¡£
        // 'Cache-Control': 'no-cache',
        // Ö¸¶¨¸ÃObject±»ÏÂÔØÊ±µÄÃû³Æ¡£
        // 'Content-Disposition': 'oss_download.txt',
        // Ö¸¶¨¸ÃObject±»ÏÂÔØÊ±µÄÄÚÈÝ±àÂë¸ñÊ½¡£
        // 'Content-Encoding': 'UTF-8',
        // Ö¸¶¨¹ýÆÚÊ±¼ä¡£
        // 'Expires': 'Wed, 08 Jul 2022 16:57:01 GMT',
        // Ö¸¶¨ObjectµÄ´æ´¢ÀàÐÍ¡£
        // 'x-oss-storage-class': 'Standard',
        // Ö¸¶¨ObjectµÄ·ÃÎÊÈ¨ÏÞ¡£
        'x-oss-object-acl': 'public-read-write',
        // ÉèÖÃObjectµÄ±êÇ©£¬¿ÉÍ¬Ê±ÉèÖÃ¶à¸ö±êÇ©¡£
        // 'x-oss-tagging': 'Tag1=1&Tag2=2',
        // Ö¸¶¨CopyObject²Ù×÷Ê±ÊÇ·ñ¸²¸ÇÍ¬ÃûÄ¿±êObject¡£´Ë´¦ÉèÖÃÎªtrue£¬±íÊ¾½ûÖ¹¸²¸ÇÍ¬ÃûObject¡£
        // 'x-oss-forbid-overwrite': 'true',
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
        // ÌîÐ´ObjectÍêÕûÂ·¾¶¡£ObjectÍêÕûÂ·¾¶ÖÐ²»ÄÜ°üº¬BucketÃû³Æ¡£
        // Äú¿ÉÒÔÍ¨¹ý×Ô¶¨ÒåÎÄ¼þÃû£¨ÀýÈçexampleobject.txt£©»òÎÄ¼þÍêÕûÂ·¾¶£¨ÀýÈçexampledir/exampleobject.txt£©µÄÐÎÊ½ÊµÏÖ½«Êý¾ÝÉÏ´«µ½µ±Ç°Bucket»òBucketÖÐµÄÖ¸¶¨Ä¿Â¼¡£
        // data¶ÔÏó¿ÉÒÔ×Ô¶¨ÒåÎªfile¶ÔÏó¡¢BlobÊý¾Ý»òÕßOSS Buffer¡£
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