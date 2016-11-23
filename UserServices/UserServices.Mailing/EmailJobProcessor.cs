//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email jobs processor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using UserServices.Worker.Common;

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Lomo.Logging;
    using LoMo.Templating;
    using Microsoft.WindowsAzure.Storage;
    //using Microsoft.WindowsAzure.Storage;
    using Newtonsoft.Json;
    using Users.Dal;
    
    /// <summary>
    /// The email jobs processor
    /// </summary>
    public class EmailJobProcessor : JobProcessor
    {
        #region Data Members

        /// <summary>
        /// The agent id.
        /// </summary>
        private readonly string _agentId;

        /// <summary>
        /// The jobs queue.
        /// </summary>
        private readonly IJobsQueue<EmailCargo> _jobsQueue;

        /// <summary>
        /// Priority email jobs queue
        /// </summary>
        private readonly IPriorityEmailJobsQueue<PriorityEmailCargo> _priorityEmailJobsQueue;

        private readonly Dictionary<Type, object> _jobHandlers;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailJobProcessor"/> class.
        /// </summary>
        /// <param name="agentId">
        /// The agent id.
        /// </param>
        /// <param name="jobsQueue">
        /// The jobs queue.
        /// </param>
        /// <param name="priorityEmailJobsQueue">priority email jobs queue</param>
        /// <param name="jobHandlers">handlers for email jobs</param>
        public EmailJobProcessor(
            string agentId,
            IJobsQueue<EmailCargo> jobsQueue,
            IPriorityEmailJobsQueue<PriorityEmailCargo> priorityEmailJobsQueue,
            Dictionary<Type, object> jobHandlers
           )
        {
            this._agentId = agentId;
            this._jobsQueue = jobsQueue;
            this._priorityEmailJobsQueue = priorityEmailJobsQueue;
            this._jobHandlers = jobHandlers;
        }

        /// <summary>
        /// Email Job processor worker method
        /// </summary>
        /// <param name="ct">
        /// The cancellation token. If ct is null the work will continue for ever otherwise it will continue until a cancel request
        /// </param>
        public override void DoWork(CancellationToken? ct)
        {
            Log.Info(EventCode.EmailAgentStarted, "Starting Email Agent. Agent Id: {0}", this._agentId);
            while (!ct.HasValue || !ct.Value.IsCancellationRequested)
            {
                this.ProcessNextRequest();
            }

            Log.Info(EventCode.EmailAgentStopped, "Stop Email Agent. Agent Id: {0}", this._agentId);
        }

        #region Private Members

        /// <summary>
        /// Dequeues an item from the specified queue and processes the message
        /// </summary>
        private void ProcessNextRequest()
        {
            bool foundPriorityJob = ProcessPriorityEmailJob();
            bool foundRegularJob = ProcessRegularEmailJob();

            if (!foundPriorityJob && !foundRegularJob)
            {
                Log.Verbose("No jobs in the regular and priority email jobs queue. Agent Id: {0} is going to sleep for {1} seconds", this._agentId, SleepTimeWhenQueueEmpty.TotalSeconds);
                Thread.Sleep(SleepTimeWhenQueueEmpty);
            }
        }

        /// <summary>
        /// Dequeues the job from priority email jobs queue and passes it to the appropriate job handler
        /// </summary>
        private bool ProcessPriorityEmailJob()
        {
            PriorityEmailCargo priorityEmailCargo = null;
            PriorityQueueMessage priorityQueueMessage = null;
            bool foundJob = false;
            try
            {
                if (this._priorityEmailJobsQueue.TryDequeue(out priorityQueueMessage))
                {
                    foundJob = true;
                    Type emailCargoType = priorityQueueMessage.EmailCargo.GetType();
                    if (_jobHandlers.ContainsKey(emailCargoType))
                    {
                        priorityEmailCargo = priorityQueueMessage.EmailCargo;
                        object handler = _jobHandlers[emailCargoType];
                        Log.Verbose("Start Processing Job. Agent Id: {0}; JobInfo : {1} ", this._agentId,
                                    priorityEmailCargo.ToString());

                        if (handler is IPriorityEmailJobHandler)
                        {
                            (handler as IPriorityEmailJobHandler).Handle(priorityQueueMessage.EmailCargo);
                            Log.Verbose("Completed Processing Job. Agent Id: {0}; JobInfo : {1} ", this._agentId,
                                        priorityEmailCargo.ToString());

                            //Delete the job from the priority email jobs queue
                            this._priorityEmailJobsQueue.Delete(priorityQueueMessage.MessageId, priorityQueueMessage.PopReceipt);
                            Log.Verbose("Removed message from the priority-email jobs queue. Message Info : {0} ",
                                        priorityQueueMessage.ToString());
                        }
                        else
                        {
                            Log.Error(string.Format("Invalid handler associated with the Email Job of type {0}",
                                                    emailCargoType));
                        }
                    }
                    else
                    {
                        Log.Error(string.Format("No handler associated with the Email Job of type {0}", emailCargoType));
                    }
                }
                else
                {
                    // No jobs in the queue.
                    Log.Verbose("No jobs in the priority email jobs queue.");
                }
            }
            catch (InvalidEntityTypeException entityTypeException)
            {
                if (priorityQueueMessage != null)
                {
                    //If it's an invalid entity in the message, we will never be able to process this.
                    //Delete the job from the priority email jobs queue
                    this._priorityEmailJobsQueue.Delete(priorityQueueMessage.MessageId, priorityQueueMessage.PopReceipt);
                }
                this.HandleError(EventCode.EmailAgentUnexpectedError, entityTypeException, "Unexpected Error", this._agentId, priorityEmailCargo);
            }
            catch (JsonException jsonException)
            {
                this.HandleError(EventCode.EmailAgentJsonSerializationError, jsonException, "Serialization Error", this._agentId, priorityEmailCargo);
            }
            catch (Exception exp)
            {
                this.HandleError(EventCode.EmailAgentUnexpectedError, exp, "Unexpected Error", this._agentId, priorityEmailCargo);
            }

            return foundJob;
        }

        /// <summary>
        /// Dequeues the job from email jobs queue and passes it to the appropriate job handler
        /// </summary>
        private bool ProcessRegularEmailJob()
        {
            EmailCargo emailCargo = null;
            bool foundJob = false;
            try
            {
                if (this._jobsQueue.TryDequeue(out emailCargo))
                {
                    foundJob = true;
                    if (_jobHandlers.ContainsKey(emailCargo.GetType()))
                    {
                        object handler = _jobHandlers[emailCargo.GetType()];
                        Log.Verbose("Start Processing Job. Agent Id: {0}; JobInfo : {1} ", this._agentId, emailCargo.ToString());

                        if (handler is IEmailJobHandler)
                        {
                            (handler as IEmailJobHandler).Handle(emailCargo);
                            Log.Verbose("Completed Processing Job. Agent Id: {0}; JobInfo : {1} ", this._agentId, emailCargo.ToString());
                        }
                        else
                        {
                            Log.Error(string.Format("Invalid handler associated with the Email Job of type {0}", emailCargo.GetType()));
                        }
                    }
                    else
                    {
                        Log.Error(string.Format("No handler associated with the Email Job of type {0}", emailCargo.GetType()));
                    }
                }
                else
                {
                    // No jobs in the queue.
                    Log.Verbose("No jobs in the email jobs queue");
                }
            }
            catch (ModelContentException modelContentException)
            {
                Log.Warn(string.Format("Can't generate email model. Agent Id={0}; Job Details=[{1}]; Error={2}", this._agentId, emailCargo, modelContentException));
            }
            catch (StorageException storageClientException)
            {
                this.HandleError(EventCode.EmailAgentStorageAccessError, storageClientException, "Error Access jobs or outbound message store", this._agentId, emailCargo);
            }
            catch (JsonException jsonException)
            {
                this.HandleError(EventCode.EmailAgentJsonSerializationError, jsonException, "Serialization Error", this._agentId, emailCargo);
            }
            catch (TemplateRenderException renderException)
            {
                this.HandleError(EventCode.EmailAgentTemplateRenderingError, renderException, "Couldn't Render the data into the template", this._agentId, emailCargo);
            }
            catch (Exception exp)
            {
                this.HandleError(EventCode.EmailAgentUnexpectedError, exp, "Unexpected Error", this._agentId, emailCargo);
            }

            return foundJob;
        }

        #endregion
    }
}