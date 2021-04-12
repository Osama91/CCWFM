﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 5.0.61118.0
// 
namespace CCWFM.StyleService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="", ConfigurationName="StyleService.StyleService")]
    public interface StyleService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:StyleService/RemoveSalesOrderApproval", ReplyAction="urn:StyleService/RemoveSalesOrderApprovalResponse")]
        System.IAsyncResult BeginRemoveSalesOrderApproval(string salesOrderCode, System.AsyncCallback callback, object asyncState);
        
        int EndRemoveSalesOrderApproval(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:StyleService/InsertStyleImageFromFolder", ReplyAction="urn:StyleService/InsertStyleImageFromFolderResponse")]
        System.IAsyncResult BeginInsertStyleImageFromFolder(int tblStyle, string styleCode, System.AsyncCallback callback, object asyncState);
        
        bool EndInsertStyleImageFromFolder(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface StyleServiceChannel : CCWFM.StyleService.StyleService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RemoveSalesOrderApprovalCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public RemoveSalesOrderApprovalCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public int Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InsertStyleImageFromFolderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public InsertStyleImageFromFolderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class StyleServiceClient : System.ServiceModel.ClientBase<CCWFM.StyleService.StyleService>, CCWFM.StyleService.StyleService {
        
        private BeginOperationDelegate onBeginRemoveSalesOrderApprovalDelegate;
        
        private EndOperationDelegate onEndRemoveSalesOrderApprovalDelegate;
        
        private System.Threading.SendOrPostCallback onRemoveSalesOrderApprovalCompletedDelegate;
        
        private BeginOperationDelegate onBeginInsertStyleImageFromFolderDelegate;
        
        private EndOperationDelegate onEndInsertStyleImageFromFolderDelegate;
        
        private System.Threading.SendOrPostCallback onInsertStyleImageFromFolderCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public StyleServiceClient() {
        }
        
        public StyleServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public StyleServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public StyleServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public StyleServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<RemoveSalesOrderApprovalCompletedEventArgs> RemoveSalesOrderApprovalCompleted;
        
        public event System.EventHandler<InsertStyleImageFromFolderCompletedEventArgs> InsertStyleImageFromFolderCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult CCWFM.StyleService.StyleService.BeginRemoveSalesOrderApproval(string salesOrderCode, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginRemoveSalesOrderApproval(salesOrderCode, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        int CCWFM.StyleService.StyleService.EndRemoveSalesOrderApproval(System.IAsyncResult result) {
            return base.Channel.EndRemoveSalesOrderApproval(result);
        }
        
        private System.IAsyncResult OnBeginRemoveSalesOrderApproval(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string salesOrderCode = ((string)(inValues[0]));
            return ((CCWFM.StyleService.StyleService)(this)).BeginRemoveSalesOrderApproval(salesOrderCode, callback, asyncState);
        }
        
        private object[] OnEndRemoveSalesOrderApproval(System.IAsyncResult result) {
            int retVal = ((CCWFM.StyleService.StyleService)(this)).EndRemoveSalesOrderApproval(result);
            return new object[] {
                    retVal};
        }
        
        private void OnRemoveSalesOrderApprovalCompleted(object state) {
            if ((this.RemoveSalesOrderApprovalCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RemoveSalesOrderApprovalCompleted(this, new RemoveSalesOrderApprovalCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void RemoveSalesOrderApprovalAsync(string salesOrderCode) {
            this.RemoveSalesOrderApprovalAsync(salesOrderCode, null);
        }
        
        public void RemoveSalesOrderApprovalAsync(string salesOrderCode, object userState) {
            if ((this.onBeginRemoveSalesOrderApprovalDelegate == null)) {
                this.onBeginRemoveSalesOrderApprovalDelegate = new BeginOperationDelegate(this.OnBeginRemoveSalesOrderApproval);
            }
            if ((this.onEndRemoveSalesOrderApprovalDelegate == null)) {
                this.onEndRemoveSalesOrderApprovalDelegate = new EndOperationDelegate(this.OnEndRemoveSalesOrderApproval);
            }
            if ((this.onRemoveSalesOrderApprovalCompletedDelegate == null)) {
                this.onRemoveSalesOrderApprovalCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRemoveSalesOrderApprovalCompleted);
            }
            base.InvokeAsync(this.onBeginRemoveSalesOrderApprovalDelegate, new object[] {
                        salesOrderCode}, this.onEndRemoveSalesOrderApprovalDelegate, this.onRemoveSalesOrderApprovalCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult CCWFM.StyleService.StyleService.BeginInsertStyleImageFromFolder(int tblStyle, string styleCode, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginInsertStyleImageFromFolder(tblStyle, styleCode, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        bool CCWFM.StyleService.StyleService.EndInsertStyleImageFromFolder(System.IAsyncResult result) {
            return base.Channel.EndInsertStyleImageFromFolder(result);
        }
        
        private System.IAsyncResult OnBeginInsertStyleImageFromFolder(object[] inValues, System.AsyncCallback callback, object asyncState) {
            int tblStyle = ((int)(inValues[0]));
            string styleCode = ((string)(inValues[1]));
            return ((CCWFM.StyleService.StyleService)(this)).BeginInsertStyleImageFromFolder(tblStyle, styleCode, callback, asyncState);
        }
        
        private object[] OnEndInsertStyleImageFromFolder(System.IAsyncResult result) {
            bool retVal = ((CCWFM.StyleService.StyleService)(this)).EndInsertStyleImageFromFolder(result);
            return new object[] {
                    retVal};
        }
        
        private void OnInsertStyleImageFromFolderCompleted(object state) {
            if ((this.InsertStyleImageFromFolderCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.InsertStyleImageFromFolderCompleted(this, new InsertStyleImageFromFolderCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void InsertStyleImageFromFolderAsync(int tblStyle, string styleCode) {
            this.InsertStyleImageFromFolderAsync(tblStyle, styleCode, null);
        }
        
        public void InsertStyleImageFromFolderAsync(int tblStyle, string styleCode, object userState) {
            if ((this.onBeginInsertStyleImageFromFolderDelegate == null)) {
                this.onBeginInsertStyleImageFromFolderDelegate = new BeginOperationDelegate(this.OnBeginInsertStyleImageFromFolder);
            }
            if ((this.onEndInsertStyleImageFromFolderDelegate == null)) {
                this.onEndInsertStyleImageFromFolderDelegate = new EndOperationDelegate(this.OnEndInsertStyleImageFromFolder);
            }
            if ((this.onInsertStyleImageFromFolderCompletedDelegate == null)) {
                this.onInsertStyleImageFromFolderCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnInsertStyleImageFromFolderCompleted);
            }
            base.InvokeAsync(this.onBeginInsertStyleImageFromFolderDelegate, new object[] {
                        tblStyle,
                        styleCode}, this.onEndInsertStyleImageFromFolderDelegate, this.onInsertStyleImageFromFolderCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override CCWFM.StyleService.StyleService CreateChannel() {
            return new StyleServiceClientChannel(this);
        }
        
        private class StyleServiceClientChannel : ChannelBase<CCWFM.StyleService.StyleService>, CCWFM.StyleService.StyleService {
            
            public StyleServiceClientChannel(System.ServiceModel.ClientBase<CCWFM.StyleService.StyleService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginRemoveSalesOrderApproval(string salesOrderCode, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = salesOrderCode;
                System.IAsyncResult _result = base.BeginInvoke("RemoveSalesOrderApproval", _args, callback, asyncState);
                return _result;
            }
            
            public int EndRemoveSalesOrderApproval(System.IAsyncResult result) {
                object[] _args = new object[0];
                int _result = ((int)(base.EndInvoke("RemoveSalesOrderApproval", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginInsertStyleImageFromFolder(int tblStyle, string styleCode, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = tblStyle;
                _args[1] = styleCode;
                System.IAsyncResult _result = base.BeginInvoke("InsertStyleImageFromFolder", _args, callback, asyncState);
                return _result;
            }
            
            public bool EndInsertStyleImageFromFolder(System.IAsyncResult result) {
                object[] _args = new object[0];
                bool _result = ((bool)(base.EndInvoke("InsertStyleImageFromFolder", _args, result)));
                return _result;
            }
        }
    }
}