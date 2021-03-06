/*
 *
 * Confidential Information of Telekinesys Research Limited (t/a Havok). Not for disclosure or distribution without Havok's
 * prior written consent. This software contains code, techniques and know-how which is confidential and proprietary to Havok.
 * Product and Trade Secret source code contains trade secrets of Havok. Havok Software (C) Copyright 1999-2014 Telekinesys Research Limited t/a Havok. All Rights Reserved. Use of this software is subject to the terms of an end user license agreement.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using HavokManaged;
using CSharpFramework;
using CSharpFramework.Docking;
using CSharpFramework.Serialization;
using CSharpFramework.Math;
using CSharpFramework.Controls;
using CSharpFramework.UndoRedo;
using CSharpFramework.Scene;
using CSharpFramework.Actions;

namespace HavokEditorPlugin
{
  public partial class HavokPhysicsPanel : DockableForm
  {
    #region Class - HavokPhysicsPanelSettings

    [Serializable]
    [YamlFormatter.SerializeInline]
    public class HavokPhysicsPanelSettings : ISerializable
    {
      public HavokPhysicsPanelSettings() { }

      #region ISerializable Members

      protected HavokPhysicsPanelSettings(SerializationInfo info, StreamingContext context)
      {
        try
        {
          HavokManaged.WorldSetupSettings ws = new HavokManaged.WorldSetupSettings();
          HavokManaged.WorldRuntimeSettings wr = new HavokManaged.WorldRuntimeSettings();

          // Ensure, that all members of settings have proper default values
          HavokManaged.ManagedModule.ResetWorldSettings();
          HavokManaged.ManagedModule.GetWorldSetupSettings(ws);
          HavokManaged.ManagedModule.GetWorldRuntimeSettings(wr);

          ws.m_havokToVisionScale = (float)info.GetValue("HavokToVisionScale", typeof(float));
          if (SerializationHelper.HasElement(info, "HavokStaticGeomMode"))
            ws.m_staticGeomMode = (int)info.GetValue("HavokStaticGeomMode", typeof(int));
          if (SerializationHelper.HasElement(info, "HavokMergedStaticWeldingType"))
            ws.m_mergedStaticWeldingType = (int)info.GetValue("HavokMergedStaticWeldingType", typeof(int));
          
          wr.m_broadPhaseSizeAuto = (bool)info.GetValue("HavokAutoBroadPhase", typeof(bool));
          wr.m_broadPhaseSizeManual = (float)info.GetValue("HavokManualBroadPhaseSize", typeof(float));
          wr.m_gravityX = (float)info.GetValue("HavokGravityX", typeof(float));
          wr.m_gravityY = (float)info.GetValue("HavokGravityY", typeof(float));
          wr.m_gravityZ = (float)info.GetValue("HavokGravityZ", typeof(float));

          if (SerializationHelper.HasElement(info, "HavokDiskShapeCaching"))
            wr.m_diskShapeCaching = (bool)info.GetValue("HavokDiskShapeCaching", typeof(bool));
          else if (SerializationHelper.HasElement(info, "HavokStaticMeshCaching")) // old serialization name
            wr.m_diskShapeCaching = (bool)info.GetValue("HavokStaticMeshCaching", typeof(bool));

          if (SerializationHelper.HasElement(info, "HavokDisableConstrainedBodiesCollisions"))
          {
            wr.m_disableConstrainedBodiesCollisions = (bool)info.GetValue("HavokDisableConstrainedBodiesCollisions", typeof(bool));
          }
          if (SerializationHelper.HasElement(info, "HavokEnableLegacyCompoundShapes"))
          {
              wr.m_enableLegacyCompoundShapes = (bool)info.GetValue("HavokEnableLegacyCompoundShapes", typeof(bool));
          }

          foreach (HavokCollisionGroups_e value in Enum.GetValues(typeof(HavokCollisionGroups_e)))
          {
            if (SerializationHelper.HasElement(info, value.ToString()))
              wr.m_collisionGroupMasks[(int)value] = (UInt32)info.GetValue(value.ToString(), typeof(UInt32));
          }

          if (SerializationHelper.HasElement(info, "HavokSolverIterations"))
              wr.m_solverIterations = (int)info.GetValue("HavokSolverIterations", typeof(int));

          if (SerializationHelper.HasElement(info, "HavokSolverHardness"))
              wr.m_solverHardness = (int)info.GetValue("HavokSolverHardness", typeof(int));

          HavokManaged.ManagedModule.SetWorldSetupSettings(ws, true);
          HavokManaged.ManagedModule.SetWorldRuntimeSettings(wr);
        }
        catch
        {
        }
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
        try
        {
          HavokManaged.WorldSetupSettings ws = new HavokManaged.WorldSetupSettings();
          HavokManaged.WorldRuntimeSettings wr = new HavokManaged.WorldRuntimeSettings();

          HavokManaged.ManagedModule.GetWorldSetupSettings(ws);
          HavokManaged.ManagedModule.GetWorldRuntimeSettings(wr);

          info.AddValue("HavokToVisionScale", ws.m_havokToVisionScale);
          info.AddValue("HavokStaticGeomMode", ws.m_staticGeomMode);
          info.AddValue("HavokMergedStaticWeldingType", ws.m_mergedStaticWeldingType);
          
          info.AddValue("HavokAutoBroadPhase", wr.m_broadPhaseSizeAuto);
          info.AddValue("HavokManualBroadPhaseSize", wr.m_broadPhaseSizeManual);
          info.AddValue("HavokGravityX", wr.m_gravityX);
          info.AddValue("HavokGravityY", wr.m_gravityY);
          info.AddValue("HavokGravityZ", wr.m_gravityZ);
          info.AddValue("HavokDiskShapeCaching", wr.m_diskShapeCaching);
          info.AddValue("HavokDisableConstrainedBodiesCollisions", wr.m_disableConstrainedBodiesCollisions);
          info.AddValue("HavokEnableLegacyCompoundShapes", wr.m_enableLegacyCompoundShapes);
          foreach (HavokCollisionGroups_e value in Enum.GetValues(typeof(HavokCollisionGroups_e)))
          {
            info.AddValue(value.ToString(), wr.m_collisionGroupMasks[(int)value]);
          }

          info.AddValue("HavokSolverIterations", wr.m_solverIterations);
          info.AddValue("HavokSolverHardness", wr.m_solverHardness);
        }
        catch
        {
        }
      }

      #endregion
    }

    #endregion


    #region Member Variables

    ToolStripHelpButton _helpbutton = null;
    private HavokPhysicsSettingsWrapper _settings = null;

    #endregion

    #region Constructor

    public HavokPhysicsPanel(DockingContainer container)
      : base(container)
    {
      InitializeComponent();
      SetupCollisionDataGridView();

      GetHavokPhysicsParams();
      GetWorldRuntimeCollisionParams();
      UpdateStatus();

      // Add help button
      _helpbutton = new ToolStripHelpButton(Text);
      ToolStrip.Items.Add(_helpbutton);

      EditorManager.EditorModeChanged += new EditorModeChangedEventHandler(EditorManager_EditorModeChanged);
      EditorManager.CustomSceneSerialization += new CustomSceneSerializationEventHandler(EditorManager_CustomSceneSerialization);
      EditorManager.SceneChanged += new SceneChangedEventHandler(EditorManager_SceneChanged);
      IScene.LayerChanged += new LayerChangedEventHandler(IScene_LayerChanged);
      EditorManager.EditorSettingsChanged += EditorManager_EditorSettingsChanged;
      IScene.PropertyChanged += IScene_PropertyChanged;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      EditorManager.EditorSettingsChanged -= EditorManager_EditorSettingsChanged;
      EditorManager.EditorModeChanged -= new EditorModeChangedEventHandler(EditorManager_EditorModeChanged);
      EditorManager.CustomSceneSerialization -= new CustomSceneSerializationEventHandler(EditorManager_CustomSceneSerialization);
      EditorManager.SceneChanged -= new SceneChangedEventHandler(EditorManager_SceneChanged);
      IScene.LayerChanged -= new LayerChangedEventHandler(IScene_LayerChanged);
      IScene.PropertyChanged -= IScene_PropertyChanged;

      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #endregion

    #region Get/SetPhysicsParameter

    protected HavokPhysicsSettingsWrapper.SolverType_e getSolverType( int iters, int hardness )
    {
        if (iters < 4)
        {
            return (hardness < 1) ? HavokPhysicsSettingsWrapper.SolverType_e.Two_Iterations_Soft : ((hardness < 2) ? HavokPhysicsSettingsWrapper.SolverType_e.Two_Iterations_Medium : HavokPhysicsSettingsWrapper.SolverType_e.Two_Iterations_Hard);
        }
        else if (iters < 8)
        {
            return (hardness < 1) ? HavokPhysicsSettingsWrapper.SolverType_e.Four_Iterations_Soft : ((hardness < 2) ? HavokPhysicsSettingsWrapper.SolverType_e.Four_Iterations_Medium : HavokPhysicsSettingsWrapper.SolverType_e.Four_Iterations_Hard);
        } 
        // 8 or higher iters
        return (hardness < 1) ? HavokPhysicsSettingsWrapper.SolverType_e.Eight_Iterations_Soft : ((hardness < 2) ? HavokPhysicsSettingsWrapper.SolverType_e.Eight_Iterations_Medium : HavokPhysicsSettingsWrapper.SolverType_e.Eight_Iterations_Hard);
    }

    protected void getSolverIterations(HavokPhysicsSettingsWrapper.SolverType_e st, ref int iters, ref int hardness)
    {
        switch (st)
        {
            case HavokPhysicsSettingsWrapper.SolverType_e.Two_Iterations_Soft: iters = 2; hardness = 0; break;
            case HavokPhysicsSettingsWrapper.SolverType_e.Two_Iterations_Medium: iters = 2; hardness = 1; break;
            case HavokPhysicsSettingsWrapper.SolverType_e.Two_Iterations_Hard: iters = 2; hardness = 2; break;
            case HavokPhysicsSettingsWrapper.SolverType_e.Four_Iterations_Soft: iters = 4; hardness = 0; break;
            default: case HavokPhysicsSettingsWrapper.SolverType_e.Four_Iterations_Medium: iters = 4; hardness = 1; break;
            case HavokPhysicsSettingsWrapper.SolverType_e.Four_Iterations_Hard: iters = 4; hardness = 2; break;
            case HavokPhysicsSettingsWrapper.SolverType_e.Eight_Iterations_Soft: iters = 8; hardness = 0; break;
            case HavokPhysicsSettingsWrapper.SolverType_e.Eight_Iterations_Medium: iters = 8; hardness = 1; break;
            case HavokPhysicsSettingsWrapper.SolverType_e.Eight_Iterations_Hard: iters = 8; hardness = 2; break;
        }
    }

    protected void GetHavokPhysicsParams()
    {
      dialogCaptionBar.Description = "Version: " + HavokManaged.ManagedModule.GetVersionInfo();

      // Get settings
      HavokManaged.WorldSetupSettings ws = new HavokManaged.WorldSetupSettings();
      HavokManaged.ManagedModule.GetWorldSetupSettings(ws);
      HavokManaged.WorldRuntimeSettings wr = new HavokManaged.WorldRuntimeSettings();
      HavokManaged.ManagedModule.GetWorldRuntimeSettings(wr);

      _settings = new HavokPhysicsSettingsWrapper();
      _settings.VisionUnitsNoPrompt = ws.m_havokToVisionScale;
      _settings.StaticGeometryModeNoPrompt = (HavokPhysicsSettingsWrapper.StaticGeometryMode_e) ws.m_staticGeomMode;
      _settings.MergedStaticWeldingTypeNoPrompt = (HavokPhysicsSettingsWrapper.MergedStaticWeldingType_e) ws.m_mergedStaticWeldingType;
      _settings.BroadphaseSize = wr.m_broadPhaseSizeAuto ? HavokPhysicsSettingsWrapper.BroadphaseSize_e.BoundAllStaticMeshes : HavokPhysicsSettingsWrapper.BroadphaseSize_e.Manual;
      _settings.BroadphaseSizeManual = wr.m_broadPhaseSizeManual;
      _settings.Gravity = new Vector3F(wr.m_gravityX, wr.m_gravityY, wr.m_gravityZ);
      _settings.ConstrainedBodyCollision = !wr.m_disableConstrainedBodiesCollisions;
      _settings.LegacyCompoundShapes = wr.m_enableLegacyCompoundShapes;
      _settings.DiskShapeCaching = wr.m_diskShapeCaching;
      _settings.SolverType = getSolverType(wr.m_solverIterations, wr.m_solverHardness);
      
      // Update property grid
      PropertyGrid.SelectedObject = _settings;
    }

    protected void SetHavokPhysicsParams()
    {
      HavokManaged.WorldSetupSettings ws = new HavokManaged.WorldSetupSettings();
      HavokManaged.ManagedModule.GetWorldSetupSettings(ws);
      HavokManaged.WorldRuntimeSettings wr = new HavokManaged.WorldRuntimeSettings();
      HavokManaged.ManagedModule.GetWorldRuntimeSettings(wr);

      ws.m_havokToVisionScale = _settings.VisionUnitsNoPrompt;
      ws.m_staticGeomMode = (int)_settings.StaticGeometryModeNoPrompt;
      ws.m_mergedStaticWeldingType = (int)_settings.MergedStaticWeldingTypeNoPrompt;
      wr.m_broadPhaseSizeAuto = _settings.BroadphaseSize == HavokPhysicsSettingsWrapper.BroadphaseSize_e.BoundAllStaticMeshes;
      wr.m_broadPhaseSizeManual = _settings.BroadphaseSizeManual;
      wr.m_gravityX = _settings.Gravity.X;
      wr.m_gravityY = _settings.Gravity.Y;
      wr.m_gravityZ = _settings.Gravity.Z;
      wr.m_disableConstrainedBodiesCollisions = !_settings.ConstrainedBodyCollision;
      wr.m_enableLegacyCompoundShapes = _settings.LegacyCompoundShapes;
      wr.m_diskShapeCaching = _settings.DiskShapeCaching;
      getSolverIterations(_settings.SolverType, ref wr.m_solverIterations, ref wr.m_solverHardness);
     
      HavokManaged.ManagedModule.SetWorldSetupSettings(ws, false);
      HavokManaged.ManagedModule.SetWorldRuntimeSettings(wr);
      
      // Enable saving
      if (EditorManager.Scene != null && EditorManager.Scene.MainLayer != null)
        EditorManager.Scene.MainLayer.Dirty = true;

      // Physics
      HavokManaged.ManagedModule.EnableDebugRendering(_settings.VisualizeDynamicObjects, _settings.VisualizeRagdolls, 
        _settings.VisualizeCharacterControllers, _settings.VisualizeTriggerVolumes, _settings.VisualizeBlockerVolumes,
        _settings.VisualizeStaticObjects);
      EditorManager.ActiveView.UpdateView(false);
      PropertyGrid.Refresh();
      toolStripButton_VisDynamics.Checked = _settings.VisualizeDynamicObjects;
      toolStripButton_VisRagdolls.Checked = _settings.VisualizeRagdolls;
      toolStripButton_VisController.Checked = _settings.VisualizeCharacterControllers;
      toolStripButton_VisTrigger.Checked = _settings.VisualizeTriggerVolumes;
      toolStripButton_VisBlocker.Checked = _settings.VisualizeBlockerVolumes;
      toolStripButton_VisStatic.Checked = _settings.VisualizeStaticObjects;
    }

    protected void GetWorldRuntimeCollisionParams()
    {
      HavokManaged.WorldRuntimeSettings wr = new HavokManaged.WorldRuntimeSettings();
      HavokManaged.ManagedModule.GetWorldRuntimeSettings(wr);

      int iRow = 0;
      foreach (HavokCollisionGroups_e rowValue in Enum.GetValues(typeof(HavokCollisionGroups_e)))
      {
        uint uiMask = wr.m_collisionGroupMasks[(int)rowValue];
        int iCell = 0;
        foreach (HavokCollisionGroups_e columnValue in Enum.GetValues(typeof(HavokCollisionGroups_e)))
        {
          uint uiBit = 1u << ((int)columnValue);
          collisionDataGridView.Rows[iRow].Cells[iCell].Value = (Boolean)((uiMask & uiBit) != 0);
          iCell++;
        }
        iRow++;
      }
    }
    
    protected void SetWorldRuntimeCollisionParams()
    {
      HavokManaged.WorldRuntimeSettings wr = new HavokManaged.WorldRuntimeSettings();
      HavokManaged.ManagedModule.GetWorldRuntimeSettings(wr);

      int iRow = 0;
      foreach (HavokCollisionGroups_e rowValue in Enum.GetValues(typeof(HavokCollisionGroups_e)))
      {
        int iCell = 0;
        uint uiMask = wr.m_collisionGroupMasks[(int)rowValue];
        foreach (HavokCollisionGroups_e columnValue in Enum.GetValues(typeof(HavokCollisionGroups_e)))
        {
          if ((Boolean)collisionDataGridView.Rows[iRow].Cells[iCell].EditedFormattedValue)
            uiMask |= 1u << (int)columnValue;
          else
            uiMask &= ~(1u << (int)columnValue);
          iCell++;
        }
        iRow++;
        wr.m_collisionGroupMasks[(int)rowValue] = uiMask;
      }

      HavokManaged.ManagedModule.SetWorldRuntimeSettings(wr);

      // enable saving
      if (EditorManager.Scene != null && EditorManager.Scene.MainLayer != null)
        EditorManager.Scene.MainLayer.Dirty = true;
    }

    #endregion

    #region Helper

    private void UpdateStatus()
    {
      bool bIsSimulating = (EditorManager.EditorMode != EditorManager.Mode.EM_NONE);
      bool bHasScene = EditorManager.Scene != null;
      bool bHasMainLayer = bHasScene && EditorManager.Scene.MainLayer != null;
      bool bMainLayerLockedByUser = false;

      if (bHasMainLayer && (EditorManager.Scene.MainLayer.LockStatus == Layer.LayerLockStatus_e.LockedByUser))
        bMainLayerLockedByUser = true;

      PropertyGrid.Enabled = bHasScene && bMainLayerLockedByUser && !bIsSimulating;
      ToolStrip.Enabled = bHasScene;
      this.collisionDataGridView.Enabled = bHasScene && bMainLayerLockedByUser && !bIsSimulating;

      if (HavokManaged.ManagedModule.IsSimulationRunning())
      {
        toolStripButton_Play.Image = global::HavokEditorPlugin.Properties.Resources.physics_pause;
        toolStripButton_Play.ToolTipText = "Pause Simulation";
      }
      else
      {
        toolStripButton_Play.Image = global::HavokEditorPlugin.Properties.Resources.physics_play;
        toolStripButton_Play.ToolTipText = "Play Simulation";
      }

      toolStripButton_VisStatic.Checked = _settings.VisualizeStaticObjects;
      toolStripButton_VisDynamics.Checked = _settings.VisualizeDynamicObjects;
      toolStripButton_VisController.Checked = _settings.VisualizeCharacterControllers;
      toolStripButton_VisTrigger.Checked = _settings.VisualizeTriggerVolumes;
    }

    private void SetupCollisionDataGridView()
    {
      int iIndex = 0;
      foreach (HavokCollisionGroups_e value in Enum.GetValues(typeof(HavokCollisionGroups_e)))
      {
        // add columns
        DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
        column.CellTemplate = new DataGridViewCheckBoxCell();
        column.CellTemplate.Style.BackColor = ((iIndex % 2) == 0) ? Color.White : Color.LightGray;
        column.HeaderText = value.ToString();
        collisionDataGridView.Columns.Add(column);

        // add rows
        DataGridViewRow row = new DataGridViewRow();
        row.HeaderCell.Value = value.ToString();
        collisionDataGridView.Rows.Add(row);

        iIndex++;
      }
    }

    #endregion

    #region Custom Events

    void EditorManager_CustomSceneSerialization(object sender, CustomSceneSerializationArgs e)
    {
      if (e.CustomSceneObjects.IsSaving)
      {
        // get data from panel
        HavokPhysicsPanelSettings physSettings = new HavokPhysicsPanelSettings();
        e.CustomSceneObjects.AddObject("HavokEditorPlugin.EditorPlugin", physSettings);
      }
      else
      {
        HavokPhysicsPanelSettings physSettings = 
          (HavokPhysicsPanelSettings)e.CustomSceneObjects.FindObject("HavokEditorPlugin.EditorPlugin", typeof(HavokPhysicsPanelSettings));
        if (physSettings != null)
        {
          GetHavokPhysicsParams();
          GetWorldRuntimeCollisionParams();
        }
      }
    }

    void EditorManager_EditorModeChanged(object sender, EditorModeChangedArgs e)
    {
      GetWorldRuntimeCollisionParams(); // refresh grid reflecting any changes to the physics module collision filter
      UpdateStatus();
      
      // We didn't have a chance to configure the VDB port at Engine startup - so let's do it when changing the Editor Mode, which is the
      // earliest point at which the VDB connection is stepped anyway.
      HavokManaged.ManagedModule.SetVisualDebuggerPort(EditorManager.Settings.VisualDebuggerPort);
    }

    void EditorManager_SceneChanged(object sender, SceneChangedArgs e)
    {
      GetHavokPhysicsParams();
      UpdateStatus();
    }

    public void IScene_LayerChanged(object sender, LayerChangedArgs e)
    {
      if (e.action == LayerChangedArgs.Action.LockStatusChanged)
        UpdateStatus();
    }

    void EditorManager_EditorSettingsChanged(object sender, EditorSettingsChangedArgs e)
    {
      if (e.OldSettings.VisualDebuggerPort != e.NewSettings.VisualDebuggerPort)
      {
        HavokManaged.ManagedModule.SetVisualDebuggerPort(e.NewSettings.VisualDebuggerPort);
      }
    }

    void IScene_PropertyChanged(object sender, PropertyChangedArgs e)
    {
      if (e != null && _settings == e._component)
        SetHavokPhysicsParams();
    }


    #endregion

    #region UI Events

    delegate void setValue<T>(T value);

    private class UpdatePhysicsPropertyAction<T> : IAction
    {
        public UpdatePhysicsPropertyAction(T newValue, T oldValue, setValue<T> setter)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            _setter = setter;
        }

        public override string ShortName
        {
            get { return "Physics Collision Layer Changed"; }
        }

        public override void Do()
        {
            _setter(_newValue);
        }

        public override void Undo()
        {
            _setter(_oldValue);
        }

        T _newValue;
        T _oldValue;
        setValue<T> _setter;
    }

    private void CollisionDataGridView_CellContentClicked(object sender, DataGridViewCellEventArgs e)
    {
      object oldValue = null;
      if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
      {
          oldValue = collisionDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
      }
      collisionDataGridView.EndEdit();
      if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
      {
          // toggle corresponding counterpart, since collision has to be dis-/enabled in both collision group bitmasks
          EditorManager.Actions.Add(
              new UpdatePhysicsPropertyAction<object>(collisionDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, oldValue,
              value => { collisionDataGridView.Rows[e.ColumnIndex].Cells[e.RowIndex].Value = collisionDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value; }
              ));

        SetWorldRuntimeCollisionParams();
      }
    }

    private void CollisionDataGridView_CellContentDoubleClicked(object sender, DataGridViewCellEventArgs e)
    {
      CollisionDataGridView_CellContentClicked(sender, e);
    }

    private void disableConstrainedBodiesCollisionsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      SetWorldRuntimeCollisionParams();
    }

    private void disableConstrainedBodiesCollisionsCheckBox_CheckStateChanged(object sender, EventArgs e)
    {
      SetWorldRuntimeCollisionParams();
    }

    private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      PendingActionsManager.Execute();
      SetHavokPhysicsParams();
    }
  
    private void toolStripButton_VisDynamics_Click(object sender, EventArgs e)
    {
      EditorManager.Actions.Add(SetPropertyAction.CreateSetPropertyAction(_settings, "VisualizeDynamicObjects", !_settings.VisualizeDynamicObjects));
      SetHavokPhysicsParams();
    }

    private void toolStripButton_VisRagdolls_Click(object sender, EventArgs e)
    {
      EditorManager.Actions.Add(SetPropertyAction.CreateSetPropertyAction(_settings, "VisualizeRagdolls", !_settings.VisualizeRagdolls));
      SetHavokPhysicsParams();
    }

    private void toolStripButton_VisStatic_Click(object sender, EventArgs e)
    {
      EditorManager.Actions.Add(SetPropertyAction.CreateSetPropertyAction(_settings, "VisualizeStaticObjects", !_settings.VisualizeStaticObjects));
      SetHavokPhysicsParams();
    }

    private void toolStripButton_VisController_Click(object sender, EventArgs e)
    {
      EditorManager.Actions.Add(SetPropertyAction.CreateSetPropertyAction(_settings, "VisualizeCharacterControllers", !_settings.VisualizeCharacterControllers));
      SetHavokPhysicsParams();
    }

    private void toolStripButton_VisTrigger_Click(object sender, EventArgs e)
    {
      EditorManager.Actions.Add(SetPropertyAction.CreateSetPropertyAction(_settings, "VisualizeTriggerVolumes", !_settings.VisualizeTriggerVolumes));
      SetHavokPhysicsParams();
    }

    private void toolStripButton_VisBlocker_Click(object sender, EventArgs e)
    {
      EditorManager.Actions.Add(SetPropertyAction.CreateSetPropertyAction(_settings, "VisualizeBlockerVolumes", !_settings.VisualizeBlockerVolumes));
      SetHavokPhysicsParams();
    }

    private void toolStripButton_Play_Click(object sender, EventArgs e)
    {
      HavokManaged.ManagedModule.ToggleSimulation();
      UpdateStatus();
    }

    #endregion

  }

  [TypeConverter(typeof(UndoableObjectConverter))]
  public class HavokPhysicsSettingsWrapper : ICustomTypeDescriptor
  {
    #region Enums 

    public enum StaticGeometryMode_e
    {
      SingleInstances,
      MergedInstances
    }

    public enum MergedStaticWeldingType_e
    {
      None,
      Anticlockwise,
      Clockwise,
      Twosided
    }

    public enum BroadphaseSize_e
    {
      BoundAllStaticMeshes,
      Manual
    }

    public enum SolverType_e
    {
      Two_Iterations_Soft,
      Two_Iterations_Medium,
      Two_Iterations_Hard,
      Four_Iterations_Soft,
      Four_Iterations_Medium,
      Four_Iterations_Hard,
      Eight_Iterations_Soft,
      Eight_Iterations_Medium,
      Eight_Iterations_Hard
    }

    #endregion

    #region Category Sorting Definitions

    /// <summary>
    /// Category string
    /// </summary>
    protected const string CAT_GENERAL = "General";
    protected const string CAT_WORLDSETUP = "World Setup (Reload scene to apply changes)";
    protected const string CAT_WORLDRUNTIME = "World Runtime (Restart simulation to apply changes)";

    /// <summary>
    /// Category ID
    /// </summary>
    protected const int CATORDER_GENERAL = 0;
    protected const int CATORDER_WORLDSETUP = 1;
    protected const int CATORDER_WORLDRUNTIME = 2;

    /// <summary>
    /// Last used category ID
    /// </summary>
    public const int LAST_CATEGORY_ORDER_ID = CATORDER_WORLDRUNTIME;

    #endregion

    #region Properties
    
    [SortedCategory(CAT_GENERAL, CATORDER_GENERAL), PropertyOrder(1)]
    [DisplayName("Visualize Dynamic Objects"), Description("If enabled, the collision geometry of dynamic objects will be rendered as a green wireframe.")]
    public bool VisualizeDynamicObjects
    {
      get { return _visualizeDynamicObjects; }
      set { _visualizeDynamicObjects = value; }
    }

    [SortedCategory(CAT_GENERAL, CATORDER_GENERAL), PropertyOrder(1)]
    [DisplayName("Visualize Ragdolls"), Description("If enabled, the collision geometry of ragdolls will be rendered as a green wireframe.")]
    public bool VisualizeRagdolls
    {
        get { return _visualizeRagdolls; }
        set { _visualizeRagdolls = value; }
    }

    [SortedCategory(CAT_GENERAL, CATORDER_GENERAL), PropertyOrder(2)]
    [DisplayName("Visualize Static Objects"), Description("If enabled, the collision geometry of static objects will be rendered as a purple wireframe.")]
    public bool VisualizeStaticObjects
    {
      get { return _visualizeStaticObjects; }
      set { _visualizeStaticObjects = value; }
    }

    [SortedCategory(CAT_GENERAL, CATORDER_GENERAL), PropertyOrder(3)]
    [DisplayName("Visualize Character Controllers"), Description("If enabled, the collision geometry of character controllers will be rendered as a red wireframe.")]
    public bool VisualizeCharacterControllers
    {
      get { return _visualizeCharacterControllers; }
      set { _visualizeCharacterControllers = value; }
    }

    [SortedCategory(CAT_GENERAL, CATORDER_GENERAL), PropertyOrder(4)]
    [DisplayName("Visualize Trigger Volumes"), Description("If enabled, the collision geometry of trigger volumes will be rendered as a blue wireframe.")]
    public bool VisualizeTriggerVolumes
    {
      get { return _visualizeTriggerVolumes; }
      set { _visualizeTriggerVolumes = value; }
    }

    [SortedCategory(CAT_GENERAL, CATORDER_GENERAL), PropertyOrder(5)]
    [DisplayName("Visualize Blocker Volumes"), Description("If enabled, the collision geometry of blocker volumes will be rendered as a blue wireframe.")]
    public bool VisualizeBlockerVolumes
    {
      get { return _visualizeBlockerVolumes; }
      set { _visualizeBlockerVolumes = value; }
    }

    [SortedCategory(CAT_WORLDSETUP, CATORDER_WORLDSETUP), PropertyOrder(1)]
    [DisplayName("Vision Units"), Description("Defines how many Vision units define a meter in the world. Note: You must reload the scene for these changes to take effect.")]
    public float VisionUnits
    {
      get { return _visionUnits; }
      set
      {
        if (_visionUnits == value)
          return;

        if (EditorManager.Settings.PromptUserSceneReloadRequired)
        {
          EditorManager.ShowMessageBox("Changes to this property require the scene to be reloaded to take effect.\n You can disable this warning in the vForge settings.",
            "Physics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        _visionUnits = value;
      }
    }

    [Browsable(false)]
    public float VisionUnitsNoPrompt
    {
      get { return _visionUnits; }
      set {_visionUnits = value; }
    }

    [SortedCategory(CAT_WORLDSETUP, CATORDER_WORLDSETUP), PropertyOrder(2)]
    [DisplayName("Static Geometry Mode"), Description("Specifies how the static geometry is represented/optimized within Havok Physics (SingleInstances: Each static mesh is represented as one Physics rigid body (editable, but slow), MergedInstances: All static meshes in the scene file are baked into one Physics rigid body (not editable, but fast)).")]
    public StaticGeometryMode_e StaticGeometryMode
    {
      get { return _staticGeometryMode; }
      set
      {
        if (_staticGeometryMode == value)
          return;

        if (EditorManager.Settings.PromptUserSceneReloadRequired)
        {
          EditorManager.ShowMessageBox("Changes to this property require the scene to be reloaded to take effect.\n You can disable this warning in the vForge settings.",
            "Physics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        _staticGeometryMode = value;
      }
    }

    [Browsable(false)]
    public StaticGeometryMode_e StaticGeometryModeNoPrompt
    {
      get { return _staticGeometryMode; }
      set { _staticGeometryMode = value; }
    }

    [SortedCategory(CAT_WORLDSETUP, CATORDER_WORLDSETUP), PropertyOrder(3)]
    [DisplayName("Merged Static Welding Type"), Description("Welding is used to reduce the problem of objects bouncing while transitioning from a collision with one shape to a collision with a neighboring shape. Only considered for Shape Type Mesh. When enabled, the orientation for meshes used in Vision is usually Anticlockwise.")]
    public MergedStaticWeldingType_e MergedStaticWeldingType
    {
      get { return _mergedStaticWeldingType; }
      set
      {
        if (_mergedStaticWeldingType == value)
          return;

        if (EditorManager.Settings.PromptUserSceneReloadRequired)
        {
          EditorManager.ShowMessageBox("Changes to this property require the scene to be reloaded to take effect.\n You can disable this warning in the vForge settings.",
            "Physics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        _mergedStaticWeldingType = value;
      }
    }

    [Browsable(false)]
    public MergedStaticWeldingType_e MergedStaticWeldingTypeNoPrompt
    {
      get { return _mergedStaticWeldingType; }
      set { _mergedStaticWeldingType = value; }
    }

    [SortedCategory(CAT_WORLDRUNTIME, CATORDER_WORLDRUNTIME), PropertyOrder(1)]
    [DisplayName("Broadphase Size Mode"), Description("Bound all static meshes: The broadphase size will be automatically created from the bounding volumes of all static physics objects. Manual: The broadphase size will be set to value specified in Broadphase Manual Size.")]
    public BroadphaseSize_e BroadphaseSize
    {
      get { return _broadphaseSize; }
      set { _broadphaseSize = value; }
    }

    [SortedCategory(CAT_WORLDRUNTIME, CATORDER_WORLDRUNTIME), PropertyOrder(2)]
    [DisplayName("Broadphase Manual Size"), Description("Sets the manual broadphase size (a cube centered on the origin with the specified value as side length).")]
    public float BroadphaseSizeManual
    {
      get { return _broadphaseSizeManual; }
      set { _broadphaseSizeManual = Math.Min(value, 1e21f); }
    }

    [SortedCategory(CAT_WORLDRUNTIME, CATORDER_WORLDRUNTIME), PropertyOrder(3)]
    [DisplayName("Constrained Body Collision"), Description("Enable collisions between rigid bodies which are linked by the same constraint.")]
    public bool ConstrainedBodyCollision
    {
      get { return _constrainedBodyCollision; }
      set { _constrainedBodyCollision = value; }
    }

    [SortedCategory(CAT_WORLDRUNTIME, CATORDER_WORLDRUNTIME), PropertyOrder(4)]
    [DisplayName("Legacy Compound Shapes"), Description("Enable simulation of legacy compound shapes (i.e. extended / compressed mesh shape). Only applies to PLAYSTATION(R)3. When using Destruction in a scene, this setting should be enabled, otherwise it is recommended to disable it. Note that the legacy shapes are mutually exclusive with the static compound shape / BV compressed mesh shape for SPU.")]
    public bool LegacyCompoundShapes
    {
        get { return _legacyCompoundShapes; }
        set { _legacyCompoundShapes = value; }
    }

    [SortedCategory(CAT_WORLDRUNTIME, CATORDER_WORLDRUNTIME), PropertyOrder(5)]
    [DisplayName("Disk Shape Caching"), Description("Enable caching of Physics shapes to disk at export time and using them when loading the exported scene. When disabled, Physics shapes will have to be recreated from the collision mesh geometry on every scene load.")]
    public bool DiskShapeCaching
    {
      get { return _diskShapeCaching; }
      set { _diskShapeCaching = value; }
    }

    [SortedCategory(CAT_WORLDRUNTIME, CATORDER_WORLDRUNTIME), PropertyOrder(6)]
    [DisplayName("Gravity"), Description("Defines the amount of gravity in Vision units. Note: These changes will take effect at the the start of the next simulation.")]
    public Vector3F Gravity
    {
      get { return _gravity; }
      set { _gravity = value; }
    }

    [SortedCategory(CAT_WORLDRUNTIME, CATORDER_WORLDRUNTIME), PropertyOrder(7)]
    [DisplayName("Solver Type"), Description("Higher number of iterations will result in more accurate simulation, but increase the simulation cost. The softness of the solver controls how quickly constraints are resolved. A hard solver applies greater forces, but is less stable. By default the solver will be run 4 times with a medium hardness.")]
    public SolverType_e SolverType
    {
        get { return _solverType; }
        set { _solverType = value; }
    }

    private bool _visualizeDynamicObjects = false;
    private bool _visualizeRagdolls = false;
    private bool _visualizeCharacterControllers = false;
    private bool _visualizeTriggerVolumes = false;
    private bool _visualizeBlockerVolumes = false;
    private bool _visualizeStaticObjects = false;
    private float _visionUnits = 100.0f;
    private StaticGeometryMode_e _staticGeometryMode = StaticGeometryMode_e.SingleInstances;
    private MergedStaticWeldingType_e _mergedStaticWeldingType = MergedStaticWeldingType_e.None;
    private BroadphaseSize_e _broadphaseSize = BroadphaseSize_e.BoundAllStaticMeshes;
    private float _broadphaseSizeManual = 0.0f;
    private bool _constrainedBodyCollision = false;
    private bool _legacyCompoundShapes = false;
    private bool _diskShapeCaching = true;
    private Vector3F _gravity = new Vector3F();
    private SolverType_e _solverType = SolverType_e.Four_Iterations_Medium;
   
    #endregion

    #region ICustomTypeDescriptor Members

    public AttributeCollection GetAttributes()
    {
      return TypeDescriptor.GetAttributes(this, true);
    }

    public string GetClassName()
    {
      return TypeDescriptor.GetClassName(this, true);
    }

    public string GetComponentName()
    {
      return TypeDescriptor.GetClassName(this, true);
    }

    public TypeConverter GetConverter()
    {
      return TypeDescriptor.GetConverter(this, true);
    }

    public EventDescriptor GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(this, true);
    }

    public PropertyDescriptor GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(this, true);
    }

    public object GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(this, editorBaseType, true);
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(this, attributes, true);
    }

    public EventDescriptorCollection GetEvents()
    {
      return TypeDescriptor.GetEvents(this, true);
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
      PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);
      PropertyDescriptorCollection filtered = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
      foreach (PropertyDescriptor pd in pdc)
        if (!pd.Name.Equals("BroadphaseSizeManual") || (pd.Name.Equals("BroadphaseSizeManual") && _broadphaseSize == BroadphaseSize_e.Manual))
          filtered.Add(pd);

      return filtered;
    }

    public PropertyDescriptorCollection GetProperties()
    {
      PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, true);
      PropertyDescriptorCollection filtered = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
      foreach (PropertyDescriptor pd in pdc)
        if (!pd.Name.Equals("BroadphaseSizeManual") || (pd.Name.Equals("BroadphaseSizeManual") && _broadphaseSize == BroadphaseSize_e.Manual))
          filtered.Add(pd);

      return filtered;
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    #endregion
  }
}

/*
 * Havok SDK - Base file, BUILD(#20140618)
 * 
 * Confidential Information of Havok.  (C) Copyright 1999-2014
 * Telekinesys Research Limited t/a Havok. All Rights Reserved. The Havok
 * Logo, and the Havok buzzsaw logo are trademarks of Havok.  Title, ownership
 * rights, and intellectual property rights in the Havok software remain in
 * Havok and/or its suppliers.
 * 
 * Use of this software for evaluation purposes is subject to and indicates
 * acceptance of the End User licence Agreement for this product. A copy of
 * the license is included with this software and is also available from salesteam@havok.com.
 * 
 */
