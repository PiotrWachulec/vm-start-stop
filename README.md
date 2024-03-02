# Introduction

This repository is a part of ['100 commitów'](https://100commitow.pl/) challenge. I will work on a project for 100 days in this challenge. This project aims to have a simple solution for starting and stopping virtual machines in Azure based on the schedule. The schedule will be defined in resource tags.

# Motivation

We can save money by turning virtual machines on/off in Azure. We can turn off VMs during the night, weekends, or holidays. We can also turn on VMs during working hours. This solution can be helpful for development, testing, and staging environments.

## Example calculation

The cost of the Azure environment can be calculated with [Azure Pricing Calculator](https://azure.microsoft.com/en-us/pricing/calculator/). Of course, the end cost depends on many factors, like networking configuration, storage, etc. 

Let's assume that we have a VM with the following configuration:
- VM size: Standard D2 v5, 2 vCPUs, 8 GiB memory, 0 GiB temporary storage
- OS: Windows
- Region: Poland Central
- We are not using [Azure Hybrid Benefit](https://azure.microsoft.com/en-us/pricing/hybrid-benefit/#overview)
- We are not using [Reserved Instances](https://azure.microsoft.com/en-us/pricing/reserved-vm-instances).

- Running VM cost: €147.37/month
- If we are running the VM only 10 hours per day, 7 days per week (300h per month), the cost will be: €60.56/month - it means that we can save €86.81/month (58.8%).
- If we are running the VM only 10 hours per day, 5 days per week (220h per month), the cost will be: €44.01/month - it means that we can save €103,36/month (70%).

# Requirements

The solution needs to turn VMs on and off based on schedule within tags on the VM or RG levels. The tag on the RG level means that all VMs in the RG should be turned on/off simultaneously.
- We would avoid administrative overhead. We want to prevent secret management/rotation, manual authentication, etc.
- The solution should be monitored to see if it is working. If any problem will appear, we would like to be notified
    - Email notification
    - Slack/Discord notification
- A single solution should cover different subscriptions.

# Existing options

We have to options to achieve this goal with the existing Azure services:
- [based on Azure Functions: Start/Stop VMs v2 overview](https://learn.microsoft.com/en-us/azure/azure-functions/start-stop-vms/overview)
- [Auto-shutdown a virtual machine](https://learn.microsoft.com/en-us/azure/virtual-machines/auto-shutdown-vm?tabs=portal)

# Design 

[Miro board](https://miro.com/app/board/uXjVNmbIwqo=/?share_link_id=652932784930)