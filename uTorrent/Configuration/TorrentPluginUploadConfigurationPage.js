define(["loading", "dialogHelper",  "mainTabsManager","formDialogStyle"],
    function (loading, dialogHelper, mainTabsManager) {

        var pluginId = "b1390c15-5b4f-4038-bb58-b71b9ef4211b";

        function getTabs() {
            return [
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginDownloadingConfigurationPage'),
                    name: "Downloading"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginSeedingConfigurationPage'),
                    name: "Seeding"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginSettingsConfigurationPage'),
                    name: "Settings"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginUploadConfigurationPage'),
                    name: "Upload"
                },
                {
                    href: Dashboard.getConfigurationPageUrl('TorrentPluginPostProcessingConfigurationPage'),
                    name: "Post Processing"
                }
            ];
        }

        ApiClient.uploadTorrentUrl = function(uploadUrl) {
            var url = this.getUrl('AddTorrentUrl');
            return this.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify({
                    Url:encodeURIComponent(uploadUrl)
                }),
                contentType: 'application/json'
            });
        };
       
        async function uploadTorrentUrl(url) {
            return await ApiClient.uploadTorrentUrl(url);
        }

        return function (view) {
            view.addEventListener('viewshow', async () => {

                mainTabsManager.setTabs(this, 3, getTabs);

                var btnUpload = view.querySelector('#uploadButton');

                btnUpload.addEventListener('click', async (e) => {
                    loading.show();
                    e.preventDefault();
                    var inputUploadUrl = view.querySelector('#torrentUrl');
                    var url = inputUploadUrl.value;
                    var uploaded =  await uploadTorrentUrl(url);
                    Dashboard.processPluginConfigurationUpdateResult("TorrentAdded: " + uploaded);
                    inputUploadUrl.value = "";
                    loading.hide();
                });
            });

        }
    });