﻿define(["loading", "dialogHelper",  "mainTabsManager","formDialogStyle"],
    function(loading, dialogHelper, mainTabsManager) {

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
                },
                //{
                //    href: Dashboard.getConfigurationPageUrl('TorrentPluginRssConfigurationPage'),
                //    name: "Rss"
                //}
            ];
        }


        async function getSettings() {
            return await ApiClient.getJSON(ApiClient.getUrl("GetSettingsData"));
        }

        return function(view) {
            view.addEventListener('viewshow',
                async () => {

                    loading.show();

                    mainTabsManager.setTabs(this, 4, getTabs);

                    var config = await ApiClient.getPluginConfiguration(pluginId);

                    var inputAutoOrganizeFolder = view.querySelector('#autoOrganizeMonitoredFolderLocation');

                    var inputFinishedDownloadLocation = view.querySelector('#finishedDownloadLocation');

                    var enableTorrentUnpacking = view.querySelector('#enableTorrentUnpacking');

                    //var result;
                        
                    //try {
                    //    result = await getSettings();
                    //} catch(err) {

                    //    console.log("Make sure user name and password are correct.");

                    //}

                    //if (result) {
                        //const settings = result.settings;
                    inputFinishedDownloadLocation.value = config.FinishedDownloadsLocation;
                    inputAutoOrganizeFolder.value       = config.EmbyAutoOrganizeFolderPath;
                    enableTorrentUnpacking.checked      = config.EnableTorrentUnpacking || false;
                    //}

                    enableTorrentUnpacking.addEventListener('change', async () => {
                        config.EnableTorrentUnpacking = enableTorrentUnpacking.checked;
                        var updateResult = await ApiClient.updatePluginConfiguration(pluginId, config);
                        Dashboard.processPluginConfigurationUpdateResult(updateResult);
                    });

                    inputAutoOrganizeFolder.addEventListener('input', async () => {
                        config.EmbyAutoOrganizeFolderPath = inputAutoOrganizeFolder.value;
                        var updateResult = await ApiClient.updatePluginConfiguration(pluginId, config);
                        Dashboard.processPluginConfigurationUpdateResult(updateResult);
                    });

                    inputFinishedDownloadLocation.addEventListener('input', async () => {
                        config.FinishedDownloadsLocation = inputFinishedDownloadLocation.value;
                        var updateResult = await ApiClient.updatePluginConfiguration(pluginId, config);
                        Dashboard.processPluginConfigurationUpdateResult(updateResult);
                    });

                    loading.hide();

                });
        }
    });