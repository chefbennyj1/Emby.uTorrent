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
                },
                //{
                //    href: Dashboard.getConfigurationPageUrl('TorrentPluginRssConfigurationPage'),
                //    name: "Rss"
                //}
            ];
        }

        async function getRssFeeds() {
            const result = await ApiClient.getJSON(ApiClient.getUrl("GetRssFeeds"));
            return result;
        }

        function renderTableRowHtml(item) {
            var html = '';
            html += '<tr class="detailTableBodyRow detailTableBodyRow-shaded" id="row_' + item.MediaInfo.SortName.replace(' ', '_') + '">';
            html += '<td data-title="Name" class="detailTableBodyCell fileCell">' + item.Title + '</td>';
            html += '<td data-title="Name" class="detailTableBodyCell fileCell">' + item.MediaInfo.Resolution + '</td>';
            html += '<td data-title="Name" class="detailTableBodyCell fileCell">' + item.PubDate + '</td>';
            html += '<td data-title="Name" class="detailTableBodyCell fileCell">';
            html += '<button class="emby-button submit downloadTorrent" data-url="' + item.Link + '">Download</button>';
            html += '</td>';
            html += '<td class="detailTableBodyCell organizerButtonCell" style="whitespace:no-wrap;"></td>';
            html += '</tr>';
            return html;
        }

        function renderTableHtml(channel, id) { 
            var html = '';
            html += '<table data-table-id="' + id + '" class="tbl-' + channel.SortName.replace(' ', '_') + ' table detailTable paperList hide" style="width: 100%; padding:2em">';
            html += '<thead>                 ';
            html += '<tr style="text-align: left;">  ';
            html += '<th class="detailTableHeaderCell" data-priority="3">File Name</th> ';
            html += '<th class="detailTableHeaderCell" data-priority="1">Resolution</th> ';
            html += '<th class="detailTableHeaderCell" data-priority="3">Published</th>';
            html += '<th class="detailTableHeaderCell" data-priority="1">Action</th> ';
            html += '</tr>      ';
            html += '</thead>  ';
            html += '<tbody class="torrentResultBody">';

            channel.Items.forEach(item => {
                html += renderTableRowHtml(item);
            });

            html += '</tbody> ';
            html += '</table> ';

            return html;
        }
        return function (view) {
            view.addEventListener('viewshow', async () => {

                mainTabsManager.setTabs(this, 5, getTabs);

                var tablesContainer = view.querySelector('.tablesContainer');
                var html = '';

                var feedResult;
                try {
                    feedResult = await getRssFeeds();
                } catch (err) {
                    return; //Too many requests to the rss feed service. have to wait. 
                }

                for (let i = 0; i <= feedResult.length -1; i++) {
                    html += '<div data-expanded="false" class="feed-collapse paperList" data-id="' + i + '" style="display:flex; align-items: center; cursor:pointer">';
                    html += '<i class="md-icon expand-icon" style="margin:1em;transform: rotate(-90deg);">expand_more</i>';
                    html += '<h3>' + feedResult[i].Channel.SortName + '</h3>';
                    html += '</div>';
                    html += renderTableHtml(feedResult[i].Channel, i);
                };

                tablesContainer.innerHTML = html;

                view.querySelectorAll('.feed-collapse').forEach(feed => {
                    feed.addEventListener('click', (e) => {
                        var collapse = e.target.closest('.feed-collapse');
                        var id = collapse.dataset.id;
                        var associatedTable = view.querySelector('table[data-table-id="' + id + '"]');
                        if (associatedTable.classList.contains('hide')) {
                            associatedTable.classList.remove('hide');
                            collapse.querySelector('.expand-icon').style = 'transform:rotate(0deg);margin:1em;"';
                        } else {
                            associatedTable.classList.add('hide');
                            collapse.querySelector('.expand-icon').style = 'transform:rotate(-90deg);margin:1em;"';
                        }
                    })
                });
            });

        }
    });