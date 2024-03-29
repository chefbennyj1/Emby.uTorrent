﻿define(["loading", "dialogHelper",  "mainTabsManager","formDialogStyle"],
    function (loading, dialogHelper, mainTabsManager) {

        var pluginId = "b1390c15-5b4f-4038-bb58-b71b9ef4211b";
        var loaded;
        var providerImages = [];

        var pagination = {
            StartIndex      : 0,
            Limit           : 25,
            TotalRecordCount: 0
        }

        //Backend Sorting Enum:
        //NAME_ASCENDING  = 0,
        //NAME_DESCENDING = 1,
        //DATE_ASCENDING  = 2,
        //DATE_DESCENDING = 3,
        //CONTENT_MOVIE   = 4,
        //CONTENT_SERIES  = 5  

        var sortBy = {
            name_ascending : 0, 
            name_descending: 1,
            date_ascending : 2,
            date_descending: 3,
            content_movie  : 4,
            content_series: 5
        }

        var currentSort = sortBy.date_descending;
        
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

        function extractedIcon(extracted) {
            switch (extracted) {
                case true : return "M12 2C6.5 2 2 6.5 2 12S6.5 22 12 22 22 17.5 22 12 17.5 2 12 2M10 17L5 12L6.41 10.59L10 14.17L17.59 6.58L19 8L10 17Z";
                case false: return "M13,2V4C17.39,4.54 20.5,8.53 19.96,12.92C19.5,16.56 16.64,19.43 13,19.88V21.88C18.5,21.28 22.45,16.34 21.85,10.85C21.33,6.19 17.66,2.5 13,2M11,2C9.04,2.18 7.19,2.95 5.67,4.2L7.1,5.74C8.22,4.84 9.57,4.26 11,4.06V2.06M4.26,5.67C3,7.19 2.24,9.04 2.05,11H4.05C4.24,9.58 4.8,8.23 5.69,7.1L4.26,5.67M2.06,13C2.26,14.96 3.03,16.81 4.27,18.33L5.69,16.9C4.81,15.77 4.24,14.42 4.06,13H2.06M7.06,18.37L5.67,19.74C7.18,21 9.04,21.79 11,22V20C9.58,19.82 8.23,19.25 7.1,18.37H7.06M13,13V7H11V13H13M13,17V15H11V17H13Z";
            }
        }

        
        function getResultContentTypeIcon(type) {
            switch (type) {
            case "UNKNOWN": return '';
            case "MOVIE":
                return "M14.75 5.46L12 1.93L13.97 1.54L16.71 5.07L14.75 5.46M21.62 4.1L20.84 .18L16.91 .96L19.65 4.5L21.62 4.1M11.81 6.05L9.07 2.5L7.1 2.91L9.85 6.44L11.81 6.05M2 8V18C2 19.11 2.9 20 4 20H20C21.11 20 22 19.11 22 18V8H2M4.16 3.5L3.18 3.69C2.1 3.91 1.4 4.96 1.61 6.04L2 8L6.9 7.03L4.16 3.5M11 24H13V22H11V24M7 24H9V22H7V24M15 24H17V22H15V24Z";
            case "SERIES":
                return "M8.16,3L6.75,4.41L9.34,7H4C2.89,7 2,7.89 2,9V19C2,20.11 2.89,21 4,21H20C21.11,21 22,20.11 22,19V9C22,7.89 21.11,7 20,7H14.66L17.25,4.41L15.84,3L12,6.84L8.16,3M4,9H17V19H4V9M19.5,9A1,1 0 0,1 20.5,10A1,1 0 0,1 19.5,11A1,1 0 0,1 18.5,10A1,1 0 0,1 19.5,9M19.5,12A1,1 0 0,1 20.5,13A1,1 0 0,1 19.5,14A1,1 0 0,1 18.5,13A1,1 0 0,1 19.5,12Z"; 
            case "SONG":
                return "M21,3V15.5A3.5,3.5 0 0,1 17.5,19A3.5,3.5 0 0,1 14,15.5A3.5,3.5 0 0,1 17.5,12C18.04,12 18.55,12.12 19,12.34V6.47L9,8.6V17.5A3.5,3.5 0 0,1 5.5,21A3.5,3.5 0 0,1 2,17.5A3.5,3.5 0 0,1 5.5,14C6.04,14 6.55,14.12 7,14.34V6L21,3Z";
            }
        }

        async function getImage(name, type, year = 0) {

            const options = {
                SearchInfo: {
                    Name: name
                }
            };
            if (year > 0) {
                options.SearchInfo.Year = year;
            }

            const url = ApiClient._serverAddress + '/emby/' + 'Items/RemoteSearch/' + type + '?api_key=' + ApiClient._serverInfo.AccessToken;
            const rawResponse = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(options)
            });
            const content = await rawResponse.json();
            
            return content[0];
        }

        function injectImages(view, torrents) {
            try {
                const rows = view.querySelectorAll('tr');
                rows.forEach(async row => {
                    var img = row.querySelector('img');
                    if (img) {
                        var hash = img.id.split('img_')[1];
                        var torrent = torrents.filter(t => t.Hash == hash)[0];
                        if (torrent) {
                            if (!providerImages.filter(p => p.Name.toLowerCase() == torrent.MediaInfo.SortName.toLowerCase())[0]) {
                                const providerData = await getImage(torrent.MediaInfo.SortName, torrent.MediaInfo.MediaType.toLowerCase(), torrent.MediaInfo.Year);
                                providerImages.push({
                                    Name: torrent.MediaInfo.SortName,
                                    ImageUrl: providerData.ImageUrl
                                });
                            }
                            //place the image in the appropriate row.
                            img.src = providerImages.filter(p => p.Name.toLowerCase() == torrent.MediaInfo.SortName.toLowerCase())[0].ImageUrl;
                        }
                    }
                });
            } catch (err) {

            }
        }

        function renderTableRowHtml(torrent, updateRow) {
            var html = '';
            html += '<tr class="detailTableBodyRow detailTableBodyRow-shaded" id="' + torrent.Hash + '">';
            html += '<td class="detailTableBodyCell" data-title="Status">';
            html += '<svg style="width:24px;height:24px" viewBox="0 0 24 24">';

            //Oh we're doing this.
            //Status can be "started" when seeding... ugh!
            //If the progress is 1000 we are seeding. We have to change the Status. and recall a method to find its actual status properly.
            //This can not be done on the backend easily. We must figure it out here.
            var findStatus = function (torrent, updateRow) {
                switch (torrent.Status) {
                    //play icon
                    case 'started':
                        {
                            if (torrent.Progress != 1000) { //definitely download this file.
                                html += '<path fill="var(--theme-primary-color)" d="M8,5.14V19.14L19,12.14L8,5.14Z" />';
                                break;
                            } else { //definitely not download this file. Seeding it! Set it to the queue icon. just shut up about it.
                                torrent.Status = "queued";
                                findStatus(torrent, updateRow);
                                break;
                            }
                        }
                    //stop icon
                    case 'stopped':
                        html += '<path fill="currentColor" d="M18,18H6V6H18V18Z" />';
                        break;
                    //pause icon
                    case 'paused':
                        html +=
                            '<path fill="currentColor" d="M22 12C22 6.46 17.54 2 12 2C10.83 2 9.7 2.19 8.62 2.56L9.32 4.5C10.17 4.16 11.06 3.97 12 3.97C16.41 3.97 20.03 7.59 20.03 12C20.03 16.41 16.41 20.03 12 20.03C7.59 20.03 3.97 16.41 3.97 12C3.97 11.06 4.16 10.12 4.5 9.28L2.56 8.62C2.19 9.7 2 10.83 2 12C2 17.54 6.46 22 12 22C17.54 22 22 17.54 22 12M5.47 7C4.68 7 3.97 6.32 3.97 5.47C3.97 4.68 4.68 3.97 5.47 3.97C6.32 3.97 7 4.68 7 5.47C7 6.32 6.32 7 5.47 7M9 9H11V15H9M13 9H15V15H13" />';
                        break;
                    //sync icon
                    case 're-check':
                        html +=
                            '<path fill="goldenrod" d="M12,18A6,6 0 0,1 6,12C6,11 6.25,10.03 6.7,9.2L5.24,7.74C4.46,8.97 4,10.43 4,12A8,8 0 0,0 12,20V23L16,19L12,15M12,4V1L8,5L12,9V6A6,6 0 0,1 18,12C18,13 17.75,13.97 17.3,14.8L18.76,16.26C19.54,15.03 20,13.57 20,12A8,8 0 0,0 12,4Z" />';
                        break;
                    case "seeding [F]":
                    case "queued":
                        {
                            if (torrent.Progress == 1000) {
                                //seeding
                                //network computer connected icon 
                                html += ' <path fill="' + (updateRow ? 'var(--theme-primary-color)' : 'currentColor') +
                                    '" d="M4,1C2.89,1 2,1.89 2,3V7C2,8.11 2.89,9 4,9H1V11H13V9H10C11.11,9 12,8.11 12,7V3C12,1.89 11.11,1 10,1H4M4,3H10V7H4V3M3,12V14H5V12H3M14,13C12.89,13 12,13.89 12,15V19C12,20.11 12.89,21 14,21H11V23H23V21H20C21.11,21 22,20.11 22,19V15C22,13.89 21.11,13 20,13H14M3,15V17H5V15H3M14,15H20V19H14V15M3,18V20H5V18H3M6,18V20H8V18H6M9,18V20H11V18H9Z" />';
                            } else {
                                //waiting to start - clock icon
                                html +=
                                    '<path fill="currentColor" d="M12 20C16.4 20 20 16.4 20 12S16.4 4 12 4 4 7.6 4 12 7.6 20 12 20M12 2C17.5 2 22 6.5 22 12S17.5 22 12 22C6.5 22 2 17.5 2 12C2 6.5 6.5 2 12 2M17 13.9L16.3 15.2L11 12.3V7H12.5V11.4L17 13.9Z" />';
                            }
                            break;
                        }
                }
            }

            findStatus(torrent, updateRow);

            html += '</svg>';
            html += '</td>';
            //Banner/Name - Only show images for tv shows. The poster image doesn't fit the style in the layout with all the banners.
            html += '<td data-title="Name" class="detailTableBodyCell fileCell">';

            //if (torrent.MediaInfo.MediaType && torrent.MediaInfo.MediaType == "SERIES") {
            //html += '<div style="max-height:7em; overflow:hidden">';
            //html += '<img style="height:100%" id="img_' + torrent.Hash + '"/>';
            //html += '</div>';
            //} else {
            html += torrent.MediaInfo ? torrent.MediaInfo.SortName : "";
            //}
           
            html += '</td>';
            
            //Content Type  Icon
            html += '<td data-title="Content" class="detailTableBodyCell fileCell">';
            html += '<svg style="height:24px" viewBox="0 0 24 24">';
            html += '<path fill="var(--theme-primary-color)" d="';
            html += torrent.MediaInfo ? getResultContentTypeIcon(torrent.MediaInfo.MediaType) : "";
            html += ' "/>';
            html += '</svg>';
            html += '</td>';
            html += '<td data-title="Resolution" class="detailTableBodyCell fileCell">' + (torrent.MediaInfo ? torrent.MediaInfo.Resolution : "") + '</td>';
            
            //FileName
            html += '<td data-title="File Name" class="detailTableBodyCell fileCell">' + torrent.Name + '</td>';
            //Extension
            //html += '<td data-title="Size" class="detailTableBodyCell fileCell">' + torrent.Extension + '</td>';
            
            //Size
            html += '<td data-title="Size" class="detailTableBodyCell fileCell">' + torrent.Size + '</td>';
            
            //Speed
            html += '<td data-title="Speed" class="detailTableBodyCell fileCell">' + torrent.DownloadSpeedFriendly + '/s</td>';
            
            //Completed
            html += '<td data-title="Complete" class="detailTableBodyCell fileCell"><span>' + (torrent.Progress / 10) + '%</span></td>';
            
            //Extracted
            html += '<td data-title="Extracted" class="detailTableBodyCell fileCell">';
            html += '<svg style="width:24px;height:24px" viewBox="0 0 24 24">';
            html += '<path fill="var(--theme-primary-color)" d="';
            html += extractedIcon(torrent.Extracted);
            html += '" />';
            html += '</svg>';
            html += '</td>';
            
            //Ratio
            html += '<td data-title="Ratio" class="detailTableBodyCell fileCell">';
            html += '<svg style="width:24px;height:24px" viewBox="0 0 24 24">';
            if (torrent.Ratio / 100 > 1) {
                html += '<path fill="var(--theme-primary-color)" d="M19,19H5V8H19M19,3H18V1H16V3H8V1H6V3H5C3.89,3 3,3.9 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M16.53,11.06L15.47,10L10.59,14.88L8.47,12.76L7.41,13.82L10.59,17L16.53,11.06Z" />';
            } else {
                html += '<path fill="goldenrod" d="M15,13H16.5V15.82L18.94,17.23L18.19,18.53L15,16.69V13M19,8H5V19H9.67C9.24,18.09 9,17.07 9,16A7,7 0 0,1 16,9C17.07,9 18.09,9.24 19,9.67V8M5,21C3.89,21 3,20.1 3,19V5C3,3.89 3.89,3 5,3H6V1H8V3H16V1H18V3H19A2,2 0 0,1 21,5V11.1C22.24,12.36 23,14.09 23,16A7,7 0 0,1 16,23C14.09,23 12.36,22.24 11.1,21H5M16,11.15A4.85,4.85 0 0,0 11.15,16C11.15,18.68 13.32,20.85 16,20.85A4.85,4.85 0 0,0 20.85,16C20.85,13.32 18.68,11.15 16,11.15Z" />';
            }
            html += '</svg>';
            html += '</td>';
            
            //Date Added
            html += '<td data-title="Date Added" class="detailTableBodyCell fileCell">' + torrent.AddedDate + '</td>';
            //Spacer
            html += '<td class="detailTableBodyCell organizerButtonCell" style="whitespace:no-wrap;"></td>';

            html += '</tr>';
            return html;
        }

        function getTorrentResultTableHtml(torrents, view) {
            var html = '';
            torrents.forEach(async torrent => {
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
            const result = await ApiClient.getJSON(ApiClient.getUrl("GetTorrentData?StartIndex=" + pagination.StartIndex + "&Limit=" + pagination.Limit + '&SortBy=' + currentSort + ''));
            return result;
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

           
            view.querySelector('.torrentResultBody').innerHTML = getTorrentResultTableHtml(results.torrents, view);

            //injectImages(view, results.torrents);

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

                var result;
                try {
                    result = await getTorrentUpdates();
                } catch (err) {
                    console.log(err);
                    return;
                }

                var table = view.querySelector('.torrentResultBody');

                //reset svg colors to default
                var paths = table.querySelectorAll('td[data-title="Status"] svg path');
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
                        //injectImages(view, result.torrents);
                    }
                }
                monitorTorrents(view);

            }, 5000);
        }

        return function (view) {
            view.addEventListener('viewshow',
                async () => {

                    loading.show();

                    mainTabsManager.setTabs(this, 1, getTabs);

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

                    //Sort by name
                    view.querySelector('.name_sort').addEventListener('click', async (e) => {
                        e.preventDefault();
                        if (currentSort === sortBy.name_ascending) {
                            currentSort = sortBy.name_descending;
                            results = await getUTorrentData();
                            await updatePageData(view, config, results);

                           
                            return;
                        }
                 
                        currentSort = sortBy.name_ascending;
                        results = await getUTorrentData();
                        await updatePageData(view, config, results);
                        
                    });

                    //Sort by date added - Default
                    view.querySelector('.date_sort').addEventListener('click', async (e) => {
                        e.preventDefault();
                        if (currentSort === sortBy.date_ascending) {
                            currentSort = sortBy.date_descending;
                            results = await getUTorrentData();
                            await updatePageData(view, config, results);
                           
                            return;
                        }
                        currentSort = sortBy.date_ascending;
                        results = await getUTorrentData();
                        await updatePageData(view, config, results);
                        
                    });

                    //SOrt by content type (movie/tv_show)
                    view.querySelector('.content_sort').addEventListener('click', async (e) => {
                        e.preventDefault();
                        if (currentSort === sortBy.content_movie) {
                            currentSort = sortBy.content_series;
                            results = await getUTorrentData();
                            await updatePageData(view, config, results);
                            
                            return;
                        }

                        currentSort = sortBy.content_movie;
                        results = await getUTorrentData();
                        await updatePageData(view, config, results);
                       
                    });

                });
            view.addEventListener('viewhide',
                () => {
                    loaded = false;
                });
        }
        
    });