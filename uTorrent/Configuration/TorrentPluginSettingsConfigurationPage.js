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

        async function getSettings() {
            return await ApiClient.getJSON(ApiClient.getUrl("GetSettingsData"));
        }

        async function setSettings(setting, value) {
            return await ApiClient.getJSON(ApiClient.getUrl("SetSettingsData?SettingName=" + setting + "&SettingValue=" + value));
        }

        function loadSelectNumericOptions(increments, total) {
            var html = '';
            html += '<option value="0">Unlimited</option>';
            for (var i = increments; i <= total; i += increments) {
                html += '<option value="' + i + '">' + i + '</option>';
            }
            return html;
        }

        return function (view) {
            view.addEventListener('viewshow', async () => {

                    mainTabsManager.setTabs(this, 2, getTabs);

                    var config = await ApiClient.getPluginConfiguration(pluginId);

                    if (config.userName) {

                        view.querySelector('#user').value                     = config.userName;
                        view.querySelector('#pass').value                     = config.password;
                        view.querySelector('#ip').value                       = config.ipAddress;
                        view.querySelector('#port').value                     = config.port;
                        view.querySelector('#finishedDownloadLocation').value = config.FinishedDownloadsLocation;

                        var activeDownloads = view.querySelector('#selectNumActiveDownloads');
                        var activeTorrents = view.querySelector('#selectNumActiveTorrents');
                        var upload = view.querySelector('#selectMaxUpload');
                        var download = view.querySelector('#selectMaxDownload');
                        var seedRatio = view.querySelector('#seedRatio');

                        var result;
                        
                        try {
                            result = await getSettings();
                        } catch(err) {

                            console.log("Make sure user name and password are correct.");

                        }
                        
                        if (result) {

                            var settings = result.settings;

                            console.log(settings[26]); //UP
                            console.log(settings[25]); //DL

                            upload.innerHTML          = loadSelectNumericOptions(5, 1000);
                            download.innerHTML        = loadSelectNumericOptions(5, 1000);
                            activeDownloads.innerHTML = loadSelectNumericOptions(1, 50);
                            activeTorrents.innerHTML  = loadSelectNumericOptions(1, 50);

                            var settingsDownloadSpeed       = settings[25];
                            var settingsUploadSpeed         = settings[26];
                            var settingsActiveDownloadCount = settings[52];
                            var settingsActiveTorrentsCount = settings[51];
                            var stateCommand                = settings[72];
                            var settingsSeedRatio           = settings[56];

                            console.log(stateCommand[2]);

                            download.value = settingsDownloadSpeed[2];
                            upload.value = settingsUploadSpeed[2];
                            activeDownloads.value = settingsActiveDownloadCount[2];
                            activeTorrents.value = settingsActiveTorrentsCount[2];
                            seedRatio.value = settingsSeedRatio[2];
                        }

                        view.querySelector('#selectMaxUpload').addEventListener('change',
                            async (e) => {
                                var result = await setSettings("max_ul_rate", e.target.value);
                                console.log("max_ul_rate " + e.target.value);
                                console.log(result.status);
                            });

                        view.querySelector('#selectMaxDownload').addEventListener('change',
                            async (e) => {
                                var result = await setSettings("max_dl_rate", e.target.value);
                                console.log("max_dl_rate " + e.target.value);
                                console.log(result.status);
                            });

                        view.querySelector('#selectNumActiveDownloads').addEventListener('change',
                            async (e) => {
                                var result = await setSettings("max_active_downloads", e.target.value);
                                console.log("max_active_downloads " + e.target.value);
                                console.log(result.status);
                            });

                        view.querySelector('#selectNumActiveTorrents').addEventListener('change',
                            async (e) => {
                                var result = await setSettings("max_active_torrent", e.target.value);
                                console.log("max_active_torrent " + e.target.value);
                                console.log(result.status);
                            });

                        view.querySelector('#seedRatio').addEventListener('input',
                            async (e) => {
                                var result = await setSettings("seed_ratio", e.target.value);
                                console.log("seed_ratio " + e.target.value);
                                console.log(result.status);
                            });

                        //ApiClient._webSocket.addEventListener('message',
                        //    function(message) {
                        //        var json = JSON.parse(message.data);
                        //        if (json.MessageType === "TorrentUpdate") {
                        //            //updateTorrentResultTable(view, config, json);
                        //            console.log(json.Data);
                        //        }
                        //    }); 
                    }


                    view.querySelector('#saveButton').addEventListener('click',
                        async () => {

                            config.userName                  = view.querySelector('#user').value;
                            config.password                  = view.querySelector('#pass').value;
                            config.ipAddress                 = view.querySelector('#ip').value;
                            config.port                      = view.querySelector('#port').value;
                            config.FinishedDownloadsLocation = view.querySelector('#finishedDownloadLocation').value;

                            var updateResult = await ApiClient.updatePluginConfiguration(pluginId, config);
                            Dashboard.processPluginConfigurationUpdateResult(updateResult);

                        });
            });

        }
    });