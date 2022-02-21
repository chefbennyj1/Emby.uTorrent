                                                                                               define(["loading", "dialogHelper",  "mainTabsManager","formDialogStyle"],
    function (loading, dialogHelper, mainTabsManager) {

        var pluginId = "b1390c15-5b4f-4038-bb58-b71b9ef4211b";
        var loaded;
        var pagination = {
            StartIndex      : 0,
            Limit           : 25,
            TotalRecordCount: 0
        }


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

        function getPagingHtml() {

            var html = '';
            html += '<div class="listPaging">';

            const recordEnd = pagination.StartIndex + pagination.Limit > pagination.TotalRecordCount ? pagination.TotalRecordCount : pagination.StartIndex + pagination.Limit;
            html += '<span style="vertical-align:middle;">';
            html += pagination.StartIndex + 1 + '-' + recordEnd + ' of ' + pagination.TotalRecordCount;

            html += '</span>';

            html += '<div style="display:inline-block;">';

            html += '<button is="paper-icon-button-light" class="btnPreviousPage autoSize" ' + (pagination.StartIndex ? '' : 'disabled') + '><i class="md-icon">&#xE5C4;</i></button>';
            html += '<button is="paper-icon-button-light" class="btnNextPage autoSize" ' + (pagination.StartIndex + pagination.Limit >= pagination.TotalRecordCount ? 'disabled' : '') + '><i class="md-icon">&#xE5C8;</i></button>';

            html += '</div>';


            html += '</div>';

            return html;
        }

        function renderTableRowHtml(torrent, updateRow) {
            var html = '';
            html += '<tr class="detailTableBodyRow detailTableBodyRow-shaded" id="' + torrent.Hash + '">';
            html += '<td class="detailTableBodyCell" data-title="Status">';
            html += '<svg style="width:24px;height:24px" viewBox="0 0 24 24">';

            switch (torrent.Status) {
                //play icon
                case 'started': html += '<path fill="var(--theme-primary-color)" d="M8,5.14V19.14L19,12.14L8,5.14Z" />';   
                    break;
                //stop icon
                case 'stopped': html += '<path fill="currentColor" d="M18,18H6V6H18V18Z" />'; 
                    break;
                //pause icon
                case 'paused': html += '<path fill="currentColor" d="M22 12C22 6.46 17.54 2 12 2C10.83 2 9.7 2.19 8.62 2.56L9.32 4.5C10.17 4.16 11.06 3.97 12 3.97C16.41 3.97 20.03 7.59 20.03 12C20.03 16.41 16.41 20.03 12 20.03C7.59 20.03 3.97 16.41 3.97 12C3.97 11.06 4.16 10.12 4.5 9.28L2.56 8.62C2.19 9.7 2 10.83 2 12C2 17.54 6.46 22 12 22C17.54 22 22 17.54 22 12M5.47 7C4.68 7 3.97 6.32 3.97 5.47C3.97 4.68 4.68 3.97 5.47 3.97C6.32 3.97 7 4.68 7 5.47C7 6.32 6.32 7 5.47 7M9 9H11V15H9M13 9H15V15H13" />';
                    break;
                //sync icon
                case 're-check': html += '<path fill="goldenrod" d="M12,18A6,6 0 0,1 6,12C6,11 6.25,10.03 6.7,9.2L5.24,7.74C4.46,8.97 4,10.43 4,12A8,8 0 0,0 12,20V23L16,19L12,15M12,4V1L8,5L12,9V6A6,6 0 0,1 18,12C18,13 17.75,13.97 17.3,14.8L18.76,16.26C19.54,15.03 20,13.57 20,12A8,8 0 0,0 12,4Z" />';
                    break;
                case "queued":
                    {
                        if (torrent.Progress == 1000) {
                            //seeding
                            //network computer connected icon 
                            html += ' <path fill="' +
                                (updateRow ? 'var(--theme-primary-color)' : 'currentColor') +
                                '" d="M4,1C2.89,1 2,1.89 2,3V7C2,8.11 2.89,9 4,9H1V11H13V9H10C11.11,9 12,8.11 12,7V3C12,1.89 11.11,1 10,1H4M4,3H10V7H4V3M3,12V14H5V12H3M14,13C12.89,13 12,13.89 12,15V19C12,20.11 12.89,21 14,21H11V23H23V21H20C21.11,21 22,20.11 22,19V15C22,13.89 21.11,13 20,13H14M3,15V17H5V15H3M14,15H20V19H14V15M3,18V20H5V18H3M6,18V20H8V18H6M9,18V20H11V18H9Z" />';
                        } else {
                            //waiting to start - clock icon
                            html += '<path fill="currentColor" d="M12 20C16.4 20 20 16.4 20 12S16.4 4 12 4 4 7.6 4 12 7.6 20 12 20M12 2C17.5 2 22 6.5 22 12S17.5 22 12 22C6.5 22 2 17.5 2 12C2 6.5 6.5 2 12 2M17 13.9L16.3 15.2L11 12.3V7H12.5V11.4L17 13.9Z" />';
                        }
                        break;
                    }
                   
            }

                //}

                html += '</svg>';
                html += '</td>';

                html += '<td data-title="Name" class="detailTableBodyCell fileCell">' + torrent.Name + '</td>';
                html += '<td data-title="Size" class="detailTableBodyCell fileCell">' + torrent.Size + '</td>';
                html += '<td data-title="Speed" class="detailTableBodyCell fileCell">' + torrent.DownloadSpeedFriendly + '/s</td>';
                html += '<td data-title="Progress" class="detailTableBodyCell fileCell">';
                html += '<div style="display:flex;align-items:center;">';
                html += '<div class="taskProgressOuter" title="' + (torrent.Progress / 10) + '%" style="flex-grow:1;">';
                html += '<div class="taskProgressInner" style="width:' + (torrent.Progress / 10) + '%; height:0.3em; background-color: var(--theme-primary-color);">';
                html += '</div>';
                html += '</div>';
                html += '</div>';
                html += '</td>';
                html += '<td data-title="Complete" class="detailTableBodyCell fileCell"><span>' + (torrent.Progress / 10) + '%</span></td>';
                html += '<td data-title="Eta" class="detailTableBodyCell fileCell">' + (torrent.Eta) + '</td>';
                //html += '<td data-title="Date Added" class="detailTableBodyCell fileCell">' + torrent.AddedDate + '</td>';
                html += '<td data-title="Remove" class="detailTableBodyCell fileCell">';
                html += '<button id="btn_' + torrent.Hash + '" class="fab removeTorrent emby-button"><i class="md-icon">clear</i></button>';
                html += '</td>';

                html += '<td class="detailTableBodyCell organizerButtonCell" style="whitespace:no-wrap;"></td>';

                html += '</tr>';
                return html;
        }

        function getTorrentResultTableHtml(torrents) {
            var html = '';
            torrents.forEach(torrent => {
                html += renderTableRowHtml(torrent, false);
            });
            return html;
        }
        
        async function getTorrentUpdates() {
            const result = await ApiClient.getJSON(ApiClient.getUrl("GetTorrentDataUpdate"));
            return result;
        }
        
        function removeTorrent(hash, config) {
            return new Promise((resolve, reject) => {
                ApiClient.getJSON(ApiClient.getUrl("RemoveTorrent?Id=" + hash)).then((result) => {
                    resolve(result);
                });
            });
        }
        
        async function getUTorrentData() {
            const result = await ApiClient.getJSON(ApiClient.getUrl("GetTorrentData?StartIndex=" + pagination.StartIndex + "&Limit=" + pagination.Limit + "&IsDownloading=true"));
            return result;
        }

        function remoteControlTorrent(remoteCommand, id, config) {
            return new Promise((resolve, reject) => {
                ApiClient.getJSON(ApiClient.getUrl(
                    remoteCommand +
                    "?IpAddress=" +
                    config.ipAddress +
                    "&Port=" +
                    config.port +
                    "&UserName=" +
                    encodeURIComponent(config.userName) +
                    "&Password=" +
                    encodeURIComponent(config.password) +
                    "&Id=" +
                    id)).then((response) => {
                    resolve(response.status);
                });
            });
        }

        async function updatePageData(view, config, results) {

            const pagingContainer = view.querySelector('.pagingContainer');
            pagination.TotalRecordCount = results.TotalRecordCount;
            pagingContainer.innerHTML = '';
            pagingContainer.innerHTML += getPagingHtml();
            
            view.querySelector('.btnPreviousPage').addEventListener('click',
                async (btn) => {
                    btn.preventDefault();
                    loading.show();
                    pagingContainer.innerHTML = '';
                    pagination.StartIndex -= pagination.Limit;
                    results = await getUTorrentData();
                    await updatePageData(view, config, results);
                });

            view.querySelector('.btnNextPage').addEventListener('click',
                async (btn) => {
                    btn.preventDefault();
                    loading.show();
                    pagingContainer.innerHTML = '';
                    pagination.StartIndex += pagination.Limit;
                    results = await getUTorrentData();
                    await updatePageData(view, config, results);
                });

           
            view.querySelector('.torrentResultBody').innerHTML = getTorrentResultTableHtml(results.torrents);

            Dashboard.hideLoadingMsg();

            view.querySelectorAll('.removeTorrent').forEach(removeTorrentButton => {
                removeTorrentButton.addEventListener('click',
                    (e) => {
                        var tableRow = e.target.closest('tr');
                        tableRow.querySelector('.taskProgressInner').style = "background-color:yellow";
                        tableRow.disabled = true;
                        removeTorrent(e.target.closest('button').id, config).then(result => {
                            console.log(result.status);
                        });
                    });
            });

        }

       
        function monitorTorrents(view) {
            setTimeout(async () => {
                if (!loaded) return;
                var result = await getTorrentUpdates();
                var table = view.querySelector('.torrentResultBody');

                //reset svg colors to default
                var paths = table.querySelectorAll('svg path');
                for (let i = 0; i <= paths.length-1; i++) {
                    paths[i].setAttribute('fill', "currentColor");
                }


                for(let i = 0; i <= result.torrents.length-1; i++) {
                    var row;

                    try {
                        
                        row = document.getElementById(result.torrents[i].Hash);

                    } catch(err) {
                        console.log(err);
                    }
                    if (row) {
                        row.innerHTML = renderTableRowHtml(result.torrents[i], true);
                    }
                }
                monitorTorrents(view);

            }, 5000);
        }

        return function (view) {
            view.addEventListener('viewshow',
                async () => {

                    loading.show();

                    mainTabsManager.setTabs(this, 0, getTabs);

                    var config = await ApiClient.getPluginConfiguration(pluginId);

                    if (!config.ip && !config.port) {
                        loading.hide();
                        return;
                    }

                    var results;
                    
                    try {

                        results = await getUTorrentData();

                    } catch (err) {

                    }
                   
                    if(results) await updatePageData(view, config, results);
                    loaded = true;
                    monitorTorrents(view);

                });
            view.addEventListener('viewhide',
                () => {
                    loaded = false;
                });
        }
        
    });