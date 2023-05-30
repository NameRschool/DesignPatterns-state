﻿using Design_Pattern_Project_.BuilderItem;
using Design_Pattern_Project_.State;
using DesignPatternsProject.AbstractFactory.prop;
using DesignPatternsProject.Stock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using DesignPatternsProject.Observer;
using Design_Pattern_Project_.Strategy;

namespace Design_Pattern_Project_.Stock
{
    public class Machine
    {
        private IReport reportFormat;
        private Form1 form;
        public MenuState currentState;
        ItemDetailesState itemDetailesState;
        private PaymentState paymentState;
        public ObjectItems objectSelectedItems = new ObjectItems();
        string s = "-בתאבון";

        public Machine(Form1 form)
        {
            this.form = form;
            CreateComBox();
            currentState = new HomePageState(form);
            reportFormat = new TextReport();
            paymentState = new PaymentState(form);

        }
        private void CreateComBox()
        {
            foreach (var itemEntry in Menu.inventoryDrink)
            {
                form.comboBoxDrink.Items.Add(itemEntry.Key);
            }
            foreach (var itemEntry in Menu.inventoryPastris)
            {
                form.comboBoxPastris.Items.Add(itemEntry.Key);
            }
            foreach (var itemEntry in Menu.inventoryCupDrink)
            {
                form.comboBoxCupDrink.Items.Add(itemEntry.Key);
            }
            foreach (var itemEntry in Menu.inventorySnack)
            {
                form.comboBoxSnack.Items.Add(itemEntry.Key);
            }
        }
        public void SelecteItem(string selectedItem, Dictionary<Item, int> inventory)
        {
            foreach (var itemEntry in inventory)
            {
                Item item = itemEntry.Key;

                if (selectedItem == item.ToString())
                {
                    if (itemEntry.Value <= 0)
                    {
                        MessageBox.Show("אין מספיק במלאי, הנך מוחזר לתפריט הקודם");
                        form.homePageState.display();
                        break;
                    }
                    else
                    {
                        if (!form.itemsLabel.Visible)
                        {
                            form.itemsLabel.Visible = true;
                        }
                        // MessageBox.Show($"{itemEntry.Value}");
                        inventory[item] -= 1;
                        //  MessageBox.Show($"{inventory[item]}");
                        objectSelectedItems.Items.Add(itemEntry.Key);

                        objectSelectedItems.TotalPrice += itemEntry.Key.Price;
                        form.payment.Enabled = true;
                        form.toPayLabel.Visible = true;
                        form.toPayLabel.Text = $"{objectSelectedItems.TotalPrice:c2}";
                        form.itemsLabel.Text += "* " + item.Name.ToString();

                        if (itemEntry.Key is NotReadyItem)
                        {
                            NotReadyItem NRI = (NotReadyItem)itemEntry.Key;
                            if (NRI.DrinkBuilder != null)
                            {
                                DrinkDirector director = new DrinkDirector();
                                director.Prepare(NRI.DrinkBuilder, form);
                            }
                            else
                            {
                                form.homePageState.display();
                                form.paymentButton.Enabled = true;
                            }
                        }
                        else
                        {
                            form.homePageState.display();
                            form.paymentButton.Enabled = true;
                        }

                        form.comboBoxSnack.Enabled = false;
                        form.comboBoxCupDrink.Enabled = false;
                        form.comboBoxDrink.Enabled = false;
                        form.comboBoxPastris.Enabled = false;

                    }
                }
            }
        }
        public void TransitionTo(MenuState state)
        {
            currentState = state;
        }
        public string Display()
        {
           return currentState.display();
        }
        public string SelectItem()
        {
           return  currentState.selectItem();
        }
        public string ProcessPayment()
        {
           return currentState.processPayment();
        }
        public string EndOrder()
        {
            return currentState.endOrder();
        }
     
        public void Pay(PaymentContext paymentContext)
        {
            paymentContext.PerformPayment(objectSelectedItems.TotalPrice);
        }
        public void FinishOrder()
        {
            form.startButton.Enabled = true;
            foreach (var item in objectSelectedItems.Items)
            {
                s += $"{item.Name},";
                reportFormat.DailyUpdate(item);
            }
            MessageBox.Show(s);
            string nextState = EndOrder();
            UpdateState(nextState);
            StartAgain();
        }
        public void StartAgain()
        {
            form.paymentButton.Enabled = false;
            form.cashtextBox.Text = string.Empty;
            form.cashtextBox.Visible = false;
            form.submitButton.Visible = false;
            objectSelectedItems.TotalPrice = 0;
            objectSelectedItems.Items.Clear();
            form.itemsLabel.Visible = false;
            form.toPayLabel.Text = String.Empty;
            form.itemsLabel.Text = $"רשימת המוצרים-";
            form.payment.Enabled = false;
            form.startButton.Enabled = true;
            s = " בתאבון-  ";
        }
        public void Packaging()
        {
            form.giftButton.Enabled = false;
            form.bagButton.Enabled = false;
            form.noPackageButton.Enabled = false;
            form.toPayLabel.Text = $"{objectSelectedItems.TotalPrice:c2}";
            paymentState.processPayment();
        }
        public void Paying()
        {
            form.cashButton.Enabled = false;
            form.cardButton.Enabled = false;
        }
        public void UpdateState(string nextState)
        {
            form.selasLabel.Text = nextState;

            if (nextState == "Item Selection State Entered")
            {
                TransitionTo(new ItemDetailesState(form));
            }
            else if (nextState == "Payment Processing State Entered")
            {
                TransitionTo(new PaymentState(form));
            }
            else if (nextState == "Order Completion State Entered")
            {
                TransitionTo(new CompleteState(form));
            }
        }

    }
}
