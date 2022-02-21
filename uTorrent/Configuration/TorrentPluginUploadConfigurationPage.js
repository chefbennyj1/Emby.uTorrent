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
                }
            ];
        }

        ApiClient.uploadTorrentUrl = function(uploadUrl) {
            var url = this.getUrl('AddTorrentUrl');
            return this.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify({Url:uploadUrl}),
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
                    e.preventDefault();
                    var url = view.querySelector('#torrentUrl').value;
                    await uploadTorrentUrl(url);
                });

            });

        }
    });