Windows Azure WebJobs SDK PhluffyShuffy Sample
-----------------------------------

This sample represents a web site that allows users to upload multiple photos and get back
an image composed of all the pictures shuffled. It uses the Windows Azure WebJobs SDK for the
background tasks. 

When a new set of images is uploaded, a message is added to the "shufflerequests" queue. The
function CreateShuffle will be triggered by this message, will process the images and then
use the IBinder interface to create and write the result image into the Windows Azure Storage
container.

The Cleanup job is triggered on demand and deletes any shuffle blobs older than two days. This
job can also be deployed as a scheduled web job running in a Windows Azure Web Site.

In order to run the sample, you need to set the Windows Azure Storage connection strings in the
App.config file or environment variables.
Example (app.config):
  <add name="AzureJobsRuntime" connectionString="DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY" />
  <add name="AzureJobsData" connectionString="DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY" />
Example (environment variables):
  SET AzureJobsRuntime=DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY
  SET AzureJobsData=DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY

For more information about the WebJobs feature of Windows Azure Web Sites, 
see http://go.microsoft.com/fwlink/?LinkId=390226

