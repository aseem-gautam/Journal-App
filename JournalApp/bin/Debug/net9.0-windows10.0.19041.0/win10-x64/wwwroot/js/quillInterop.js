window.quillFunctions = {
    createQuill: function (quillElement, dotNetHelper) {
        if (!quillElement) {
            console.error("Quill element not found");
            return null;
        }

        var quill = new Quill(quillElement, {
            theme: 'snow',
            placeholder: 'Start writing your journal entry...',
            modules: {
                toolbar: [
                    [{ 'header': [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ 'color': [] }, { 'background': [] }],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    [{ 'align': [] }],
                    ['blockquote', 'code-block'],
                    ['link'],
                    ['clean']
                ]
            }
        });

        // Listen for text changes
        quill.on('text-change', function () {
            var html = quill.root.innerHTML;
            var text = quill.getText();
            dotNetHelper.invokeMethodAsync('OnContentChanged', html, text);
        });

        return quill;
    },

    getQuillContent: function (quillElement) {
        var quill = Quill.find(quillElement);
        if (quill) {
            return quill.root.innerHTML;
        }
        return '';
    },

    setQuillContent: function (quillElement, content) {
        var quill = Quill.find(quillElement);
        if (quill && content) {
            quill.root.innerHTML = content;
        }
    },

    getQuillText: function (quillElement) {
        var quill = Quill.find(quillElement);
        if (quill) {
            return quill.getText();
        }
        return '';
    }
};